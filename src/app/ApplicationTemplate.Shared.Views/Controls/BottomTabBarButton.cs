using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ApplicationTemplate.Views.Controls;

public partial class BottomTabBarButton : Button
{
	/// <summary>
	/// Gets or sets the icon style for the TabBar button.
	/// </summary>
	public Style IconStyle
	{
		get => (Style)GetValue(IconStyleProperty);
		set => SetValue(IconStyleProperty, value);
	}

	public static readonly DependencyProperty IconStyleProperty =
		DependencyProperty.Register("IconStyle", typeof(Style), typeof(BottomTabBarButton), new PropertyMetadata(default(Style)));
}
