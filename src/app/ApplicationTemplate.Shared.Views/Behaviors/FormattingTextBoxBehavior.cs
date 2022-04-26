//-:cnd:noEmit
#if NETFX_CORE || __ANDROID__ || __IOS__
//+:cnd:noEmit
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using Uno.Extensions;

namespace ApplicationTemplate.Views.Behaviors
{
	/// <summary>
	/// This text box behavior allows you to format the input text based on specified format. 
	/// The specified format follows the following convention:
	///'#' allows anycharacters
	///'A' : capital letter
	/// 'a' : lower case letter
	///'0' : digit
	///'N' : Capital alphanumeric
	///'n' : lower case alphanumeric
	///Example of formats: 
	///- (000) 000-000 = Phone number
	///- 0000 0000 0000 0000  = credit card
	///- A0A 0A0 = code postal
	/// </summary>
	public partial class FormattingTextBoxBehavior
	{
		public const char AnyCharacter = '#';
		public const char CapitalLetter = 'A';
		public const char LowerCaseLetter = 'a';
		public const char Digit = '0';
		public const char CapitalAlphaNumeric = 'N';
		public const char LowerCaseAlphaNumeric = 'n';

		/// <summary>
		/// This property is used to compare the previous text length and in the case we remove a character we dont want to format.
		/// </summary>
		private static int _previousTextLength;

		public static string GetTextFormat(DependencyObject obj)
		{
			return (string)obj.GetValue(TextFormatProperty);
		}

		public static void SetTextFormat(DependencyObject obj, string value)
		{
			obj.SetValue(TextFormatProperty, value);
		}

		public static readonly DependencyProperty TextFormatProperty =
			DependencyProperty.RegisterAttached("TextFormat", typeof(string), typeof(FormattingTextBoxBehavior), new PropertyMetadata("", OnTextFormatChanged));

		private static void OnTextFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			FormatText(d as TextBox, (string)e.NewValue);

			SetTextBoxLength(d as TextBox, (string)e.NewValue);

			OnTextFormatChangedPartial(d, e);
		}

		static partial void OnTextFormatChangedPartial(DependencyObject d, DependencyPropertyChangedEventArgs e);

		public static string GetRawText(DependencyObject obj)
		{
			return (string)obj.GetValue(TextFormatProperty);
		}

		public static void SetRawText(DependencyObject obj, string value)
		{
			string formatText = GetTextFormat(obj);
			StringBuilder sb = new StringBuilder();

			var valueLength = value?.Length ?? 0;

			for (int i = 0; i < valueLength; i++)
			{
				if (IsFormatCharacter(formatText[i]))
				{
					sb.Append(value[i]);
				}
			}

			obj.SetValue(RawTextProperty, sb.ToString());
		}

		private static bool IsFormatCharacter(char input)
		{
			return
				input == AnyCharacter ||
				input == CapitalLetter ||
				input == LowerCaseLetter ||
				input == Digit ||
				input == CapitalAlphaNumeric ||
				input == LowerCaseAlphaNumeric;
		}

		public static readonly DependencyProperty RawTextProperty =
			DependencyProperty.Register("RawText", typeof(string), typeof(FormattingTextBoxBehavior), new PropertyMetadata(string.Empty));

		public static bool GetIsEnabled(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsEnabledProperty);
		}

		public static void SetIsEnabled(DependencyObject obj, bool value)
		{
			obj.SetValue(IsEnabledProperty, value);
		}

		public static readonly DependencyProperty IsEnabledProperty =
			DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(FormattingTextBoxBehavior), new PropertyMetadata(false, OnIsEnabledChanged));

		private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue)
			{
				var textBox = d as TextBox;
				if (textBox == null)
				{
					return;
				}

#pragma warning disable Uno0001 // Uno type or member is not implemented
				textBox.Paste += OnTextBoxPasted;
#pragma warning restore Uno0001 // Uno type or member is not implemented
				textBox.TextChanged += OnTextChanged;

				var textFormat = GetTextFormat(textBox);

				if (textBox.Text.HasValue())
				{
					FormatText(textBox, textFormat);
				}

				SetTextBoxLength(textBox, textFormat);

				OnIsEnabledChangedPartial(d, e, textBox, textFormat);
			}
		}

		static partial void OnIsEnabledChangedPartial(DependencyObject d, DependencyPropertyChangedEventArgs e, TextBox textBox, string textFormat);

		private static void OnTextBoxPasted(object sender, TextControlPasteEventArgs e)
		{
			var textBox = sender as TextBox;
			if (textBox == null)
			{
				return;
			}

			FormatText(textBox, GetTextFormat(textBox));
		}

		private static void SetTextBoxLength(TextBox textBox, string textFormat)
		{
			if (textBox != null && textFormat != null)
			{
				textBox.MaxLength = textFormat.Length;
			}
		}

		private static void OnTextChanged(object sender, TextChangedEventArgs e)
		{
			var textbox = (sender as TextBox);
			if (textbox == null)
			{
				return;
			}

			SetRawText(textbox, textbox.Text);

			//If we remove characters we don't want to format.
			if (_previousTextLength > textbox.Text.Safe().Count())
			{
				_previousTextLength = textbox.Text.Safe().Count();
				return;
			}

			FormatText(textbox, GetTextFormat(textbox));
		}

		private static void FormatText(TextBox textbox, string textFormat)
		{
			if (textbox == null || !textFormat.HasValue() || textbox.Text.IsNullOrEmpty())
			{
				return;
			}

			var selectionStart = textbox.SelectionStart;
			var formattedText = FormatInput(textbox.Text, textFormat, 0, 0, ref selectionStart);
			if (textbox.Text == formattedText)
			{
				// Note: On Android, most of the time we will cut out here because the input filter will have already formatted the text. However 
				// the formatted text will occasionally be different if, eg, the user has inserted/deleted characters in the middle of the text.
				return;
			}

			textbox.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				textbox.Text = formattedText;
				textbox.SelectionStart = selectionStart;
			});

			_previousTextLength = textbox.Text.Length;
		}

		private static string FormatInput(string text, string format, int maskedIndex, int textIndex)
		{
			int discard = 0;

			return FormatInput(text, format, maskedIndex, textIndex, ref discard);
		}

		private static string FormatInput(string text, string format, int maskedIndex, int textIndex, ref int cursorIndex)
		{
			var sb = new StringBuilder();

			while (maskedIndex < format.Length && textIndex < text.Length)
			{
				var formatCharacter = format[maskedIndex];

				if (formatCharacter == AnyCharacter)
				{
					sb.Append(text[textIndex]);
					maskedIndex++;
					textIndex++;
				}
				else if (formatCharacter == CapitalAlphaNumeric || formatCharacter == LowerCaseAlphaNumeric)
				{
					var nextAlphaNumericIndex = GetNextAlphanumericIndex(text, textIndex, ref cursorIndex);

					if (nextAlphaNumericIndex != -1)
					{
						textIndex = nextAlphaNumericIndex;
						var character = text[textIndex];

						if (Char.IsDigit(character))
						{
							sb.Append(text[textIndex]);
						}
						else if (Char.IsLetter(character))
						{
							var letter = string.Empty;

							if (formatCharacter == LowerCaseAlphaNumeric)
							{
								letter = text[textIndex].ToString().ToLowerInvariant();
							}
							else if (formatCharacter == CapitalAlphaNumeric)
							{
								letter = text[textIndex].ToString().ToUpperInvariant();
							}

							sb.Append(letter);
						}

						maskedIndex++;
						textIndex++;
					}
					else
					{
						break;
					}
				}
				else if (formatCharacter == Digit)
				{
					var nextDigit = GetNextIndexOfDigit(text, textIndex, ref cursorIndex);
					if (nextDigit != -1)
					{
						textIndex = nextDigit;
						sb.Append(text[textIndex]);
						maskedIndex++;
						textIndex++;
					}
					else
					{
						break;
					}
				}
				else if (formatCharacter == CapitalLetter || formatCharacter == LowerCaseLetter)
				{
					var nextLetterIndex = GetNextCharacterIndex(text, textIndex, ref cursorIndex);
					if (nextLetterIndex != -1)
					{
						textIndex = nextLetterIndex;
						var letter = string.Empty;

						if (formatCharacter == LowerCaseLetter)
						{
							letter = text[textIndex].ToString().ToLowerInvariant();
						}
						else if (formatCharacter == CapitalLetter)
						{
							letter = text[textIndex].ToString().ToUpperInvariant();
						}

						sb.Append(letter);
						maskedIndex++;
						textIndex++;
					}
					else
					{
						break;
					}
				}
				else
				{
					if (formatCharacter == text[textIndex])
					{
						textIndex++;
					}
					else
					{
						if (textIndex < cursorIndex)
						{
							cursorIndex++;
						}
					}

					sb.Append(formatCharacter);
					maskedIndex++;
				}
			}

			// clamp cursor index to [0, length]; note: -1 isn't needed, since the position right after the last character is also valid
			cursorIndex = Math.Min(cursorIndex, sb.Length);
			return sb.ToString();
		}

		/// <summary>
		/// Check if the text insertion is conform to the specified format
		/// </summary>
		/// <param name="format">text format</param>
		/// <param name="offset">index where <paramref name="input"/> should be inserted</param>
		/// <param name="input">fragment to be inserted at <paramref name="offset"/></param>
		private static bool ValidateInput(string format, int offset, string input)
		{
			if (offset + input.Length > format.Length)
			{
				// refuse input beyond max-length (length of format)
				return false;
			}

			// start at offset and assume everything before is conform
			int formatIndex = offset, inputIndex = 0;

			// note: we don't necessary want to fully consume the input
			// especially when the last format character(s) are filler characters(default-case)
			// in such case, we accept the input and let TextChanged to modify it into the filler characters
			while (formatIndex < format.Length && inputIndex < input.Length)
			{
				switch (format[formatIndex])
				{
					case AnyCharacter:
						formatIndex++;
						inputIndex++;
						continue;

					case CapitalAlphaNumeric:
					case LowerCaseAlphaNumeric:
						if (char.IsLetterOrDigit(input[inputIndex]))
						{
							formatIndex++;
							inputIndex++;
							continue;
						}
						return false;

					case CapitalLetter:
					case LowerCaseLetter:
						if (char.IsLetter(input[inputIndex]))
						{
							formatIndex++;
							inputIndex++;
							continue;
						}
						return false;

					case Digit:
						if (char.IsDigit(input[inputIndex]))
						{
							formatIndex++;
							inputIndex++;
							continue;
						}
						return false;

					default: // filler/padding character
						if (format[formatIndex] == input[inputIndex])
						{
							// consume input at index only if it happens to match the filler character in format
							inputIndex++;
						}
						formatIndex++;
						continue;
				}
			}

			return true;
		}

		private static int GetNextIndexOfDigit(string text, int currentIndex, ref int cursorIndex)
		{
			while (currentIndex < text.Length)
			{
				if (Char.IsDigit(text[currentIndex]))
				{
					return currentIndex;
				}

				if (currentIndex < cursorIndex)
				{
					cursorIndex--;
				}

				currentIndex++;
			}

			return -1;
		}

		private static int GetNextCharacterIndex(string text, int currentIndex, ref int cursorIndex)
		{
			while (currentIndex < text.Length)
			{
				if (Char.IsLetter(text[currentIndex]))
				{
					return currentIndex;
				}

				if (currentIndex < cursorIndex)
				{
					cursorIndex--;
				}

				currentIndex++;
			}

			return -1;
		}

		private static int GetNextAlphanumericIndex(string text, int currentIndex, ref int cursorIndex)
		{
			while (currentIndex < text.Length)
			{
				if (Char.IsLetter(text[currentIndex]) || Char.IsDigit(text[currentIndex]))
				{
					return currentIndex;
				}

				if (currentIndex < cursorIndex)
				{
					cursorIndex--;
				}

				currentIndex++;
			}

			return -1;
		}
	}
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
