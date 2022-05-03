using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ApplicationTemplate.Views.Controls
{
	/// <summary>
	/// Radio button with optional icon, used in the menu.
	/// </summary>
	public partial class BottomTabBarButton : Button
	{
		/// <summary>
		/// Gets or sets the icon style on top of the RadioButton's text (Content).
		/// </summary>
		public Style IconStyle
		{
			get => (Style)GetValue(IconStyleProperty);
			set => SetValue(IconStyleProperty, value);
		}

		public static readonly DependencyProperty IconStyleProperty =
			DependencyProperty.Register("IconStyle", typeof(Style), typeof(BottomTabBarButton), new PropertyMetadata(default(Style)));
	}
}
