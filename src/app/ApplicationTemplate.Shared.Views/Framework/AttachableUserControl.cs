using Microsoft.UI.Xaml.Controls;

namespace ApplicationTemplate.Views;

/// <summary>
/// This is a workaround the fact that using attached properties on UserControl doesn't work with Uno.
/// See http://feedback.nventive.com/topics/257-usercontrol-doesnt-support-attached-properties/ for more details.
/// </summary>
public partial class AttachableUserControl : UserControl
{
}
