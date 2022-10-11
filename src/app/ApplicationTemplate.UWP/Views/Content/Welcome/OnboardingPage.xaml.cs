#if WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace ApplicationTemplate.Views.Content;

public sealed partial class OnboardingPage : Page
{
	public OnboardingPage()
	{
		this.InitializeComponent();
	}
}
