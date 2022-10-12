#if WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ApplicationTemplate.Views.Content;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class DadJokesFavoritesPage : Page
{
	public DadJokesFavoritesPage()
	{
		this.InitializeComponent();
	}
}
