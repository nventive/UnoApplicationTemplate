using System.Linq;
using Windows.UI.ViewManagement;
#if WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace ApplicationTemplate.Views.Content;

public sealed partial class Menu : AttachableUserControl
{
	public Menu()
	{
		this.InitializeComponent();
	}
}
