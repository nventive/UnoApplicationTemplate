//-:cnd:noEmit
#if __ANDROID__
//+:cnd:noEmit
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Text;
using Android.Widget;
using ApplicationTemplate.Views.Helpers;
using Java.Lang;
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
			// On Android we use the native SetFilters() method, which is much more performant than overwriting the Text property. 

			string textFormat = (string)e.NewValue;
			if (!textFormat.HasValue())
			{
				return;
			}

			var textBoxView = d.FindAllChildren<DependencyObject>().OfType<EditText>().FirstOrDefault();
			if (textBoxView != null)
			{
				textBoxView.SetFilters(GetFilters(textFormat, d as TextBox));
			}
			else
			{
				// TextBoxView hasn't been materialized yet, set filters when TextBox loads
				void loaded(object sender, RoutedEventArgs eLoaded)
				{
					MaterializeAllTemplates(sender as TextBox);
					var textBoxViewInner = (sender as TextBox).FindAllChildren<DependencyObject>().OfType<EditText>().FirstOrDefault();
					if (textBoxViewInner == null && typeof(FormattingTextBoxBehavior).Log().IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error))
					{
						typeof(FormattingTextBoxBehavior).Log().Error($"Could not find inner {nameof(EditText)}, input filters will not be applied.");
					}
					textBoxViewInner?.SetFilters(GetFilters(textFormat, sender as TextBox));
					(sender as TextBox).Loaded -= loaded;
				}
				(d as TextBox).Loaded += loaded;
			}
		}

		static partial void OnIsEnabledChangedPartial(DependencyObject d, DependencyPropertyChangedEventArgs e, TextBox textBox, string textFormat)
		{
			if (!textFormat.HasValue())
			{
				return;
			}
			var textBoxView = d.FindAllChildren<DependencyObject>().OfType<EditText>().FirstOrDefault();
			textBoxView?.SetFilters(GetFilters(textFormat, textBox));
		}

		private static Android.Text.IInputFilter[] GetFilters(string textFormat, TextBox targetTextBox)
		{
			if (!textFormat.HasValue())
			{
				return new Android.Text.IInputFilter[0];
			}

			return new IInputFilter[]
			{
				new FormattingTextInputFilter(textFormat, targetTextBox),
				// Special characters filtered out from input seem to still hang around in the software keyboard's text buffer. We add the
				// length filter last, otherwise these filtered characters would count toward the length limit.
				new Android.Text.InputFilterLengthFilter(textFormat.Length)
			};
		}

		/// <summary>
		/// Force all child Controls of this view (including itself) to materialize their templates. This is a useful workaround on
		/// Android where there is a timing bug such that a child's template is frequently not materialized when the parent's Loaded
		/// event is called.
		/// </summary>
		private static void MaterializeAllTemplates(DependencyObject view)
		{
			var control = view as Control;
			do
			{
				control?.ApplyTemplate();
				control = control.FindFirstChild<Control>(includeCurrent: false);
			}
			while (control != null);
		}

		private class FormattingTextInputFilter : Java.Lang.Object, Android.Text.IInputFilter
		{
			private string _stringFormat;
			private TextBox _targetTextBox;

			public FormattingTextInputFilter(string stringFormat, TextBox targetTextBox)
			{
				_stringFormat = stringFormat;
				_targetTextBox = targetTextBox;
			}

			public Java.Lang.ICharSequence FilterFormatted(Java.Lang.ICharSequence source, int start, int end, Android.Text.ISpanned dest, int dstart, int dend)
			{
				if (source.Length() == 0)
				{
					// No changes
					return null;
				}
				var filtered = FormatInput(source.ToString(), _stringFormat, dstart, start);

				ICharSequence output;
				if (source is ISpanned spanned)
				{
					var spannable = new SpannableString(filtered);
					TextUtils.CopySpansFrom(spanned, start, System.Math.Min(end, spannable.Length()), null, spannable, 0);
					output = spannable;
				}
				else
				{
					output = new Java.Lang.String(filtered);
				}
				return output;
			}
		}
	}
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
