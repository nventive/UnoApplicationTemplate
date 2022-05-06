//-:cnd:noEmit
#if __IOS__
//+:cnd:noEmit
using System;
using ApplicationTemplate.Views.Helpers;
using Foundation;
using Microsoft.Extensions.Logging;
using UIKit;
using Uno.Extensions;
using Uno.Logging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ApplicationTemplate.Views.Behaviors
{
	public partial class FormattingTextBoxBehavior
	{
		static partial void OnTextFormatChangedPartial(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			UpdateDelegate(d, null, (string)e.NewValue);
		}

		static partial void OnIsEnabledChangedPartial(DependencyObject d, DependencyPropertyChangedEventArgs e, TextBox textBox, string textFormat)
		{
			UpdateDelegate(d, (bool)e.NewValue, textFormat);
		}

		static void UpdateDelegate(DependencyObject d, bool? enabled, string format)
		{
			var textbox = (TextBox)d;
			if (textbox.GetTemplateChild(TextBoxConstants.ContentElementPartName) != null)
			{
				UpdateDelegateImpl(textbox, enabled, format);
			}
			else
			{
				textbox.ObserveLoaded().SubscribeToElement(
					textbox,
					_ => UpdateDelegateImpl(textbox, null, null),
					e => typeof(FormattingTextBoxBehavior).Log().Error(() => "Failed to observe the textbox being loaded")
				);
			}
		}

		static void UpdateDelegateImpl(TextBox textbox, bool? enabled = null, string format = null)
		{
			var nativeView = (textbox as DependencyObject).FindFirstChild<SinglelineTextBoxView>();
			if (nativeView == null)
			{
				if (typeof(FormattingTextBoxBehavior).Log().IsEnabled(LogLevel.Error))
				{
					if ((textbox as DependencyObject).FindFirstChild<MultilineTextBoxView>() != null)
					{
						typeof(FormattingTextBoxBehavior).Log().Error($"`{nameof(MultilineTextBoxView)}` is not supported. Make sure `AcceptsReturn` is set to false and `TextWrapping` is `NoWrap`.");
					}
					else
					{
						typeof(FormattingTextBoxBehavior).Log().Error($"Could not find a native view of type `{nameof(SinglelineTextBoxView)}`.");
					}
				}

				return;
			}

			// enabled and format may be null because the value may be outdated or not provided

			if (enabled ?? GetIsEnabled(textbox))
			{
				UITextFieldDelegatingDelegate.DetourOrUpdate(nativeView, format ?? GetTextFormat(textbox));
			}
			else
			{
				(nativeView.Delegate as UITextFieldDelegatingDelegate)?.RestoreDefault();
			}
		}

		public class UITextFieldDelegatingDelegate : UITextFieldDelegate
		{
			private SinglelineTextBoxView _nativeView;
			private IUITextFieldDelegate _delegate;

			public string TextFormat { get; set; }

			public UITextFieldDelegatingDelegate(SinglelineTextBoxView nativeView)
			{
				this._nativeView = nativeView;
				this._delegate = nativeView.Delegate;
			}

			public static UITextFieldDelegatingDelegate DetourOrUpdate(SinglelineTextBoxView nativeView, string format)
			{
				if (nativeView.Delegate is UITextFieldDelegatingDelegate @delegate)
				{
					@delegate.TextFormat = format;
					return @delegate;
				}
				else if (nativeView.Delegate is SinglelineTextBoxDelegate)
				{
					var result = new UITextFieldDelegatingDelegate(nativeView);
					result.TextFormat = format;
					nativeView.Delegate = result;

					return result;
				}
				else
				{
					throw new InvalidOperationException();
				}
			}

			public void RestoreDefault()
			{
				_nativeView.Delegate = _delegate;
				_delegate = null;
				_nativeView = null;

				Dispose(true);
			}

			public override bool ShouldChangeCharacters(UITextField textField, NSRange range, string replacementString)
			{
				// backspacing / deleting
				if (string.IsNullOrEmpty(replacementString))
				{
					return DefaultBehavior();
				}

				var result = ValidateInput(TextFormat, (int)range.Location, replacementString);
				result &= DefaultBehavior();

				return result;

				bool DefaultBehavior() => _delegate.ShouldChangeCharacters(textField, range, replacementString);
			}

#region restore uno implementations
			//public override bool ShouldChangeCharacters(UITextField textField, NSRange range, string replacementString) => _delegate.ShouldChangeCharacters(textField, range, replacementString);

			public override bool ShouldReturn(UITextField textField) => _delegate.ShouldReturn(textField);

			public override bool ShouldBeginEditing(UITextField textField) => _delegate.ShouldBeginEditing(textField);

			public override void EditingStarted(UITextField textField) => _delegate.EditingStarted(textField);

			public override void EditingEnded(UITextField textField) => _delegate.EditingEnded(textField);
#endregion
		}
	}
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
