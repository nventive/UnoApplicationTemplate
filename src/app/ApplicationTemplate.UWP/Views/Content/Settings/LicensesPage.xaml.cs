#if WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace ApplicationTemplate.Views.Content;

public sealed partial class LicensesPage : Page
{
	public LicensesPage()
	{
		this.InitializeComponent();
	}
}
