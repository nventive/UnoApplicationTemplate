using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ApplicationTemplate.Views.Controls
{
	/// <summary>
	/// Radio button with optional icon, used in the menu.
	/// </summary>
	public partial class MenuRadioButton : RadioButton
	{
		/// <summary>
		/// Gets or sets the icon style on top of the RadioButton's text (Content) when radio button is unchecked.
		/// </summary>
		public Style UncheckedIconStyle
		{
			get => (Style)GetValue(UncheckedIconStyleProperty);
			set => SetValue(UncheckedIconStyleProperty, value);
		}

		public static readonly DependencyProperty UncheckedIconStyleProperty =
			DependencyProperty.Register("UncheckedIconStyle", typeof(Style), typeof(MenuRadioButton), new PropertyMetadata(default(Style)));

		/// <summary>
		/// The icon style on top of the RadioButton's text (Content) when radio button is checked.
		/// </summary>
		public Style CheckedIconStyle
		{
			get => (Style)GetValue(CheckedIconStyleProperty);
			set => SetValue(CheckedIconStyleProperty, value);
		}

		public static readonly DependencyProperty CheckedIconStyleProperty =
			DependencyProperty.Register("CheckedIconStyle", typeof(Style), typeof(MenuRadioButton), new PropertyMetadata(default(Style)));
	}
}
