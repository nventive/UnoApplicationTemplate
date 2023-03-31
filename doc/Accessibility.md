# Accessibility

## SimpleAccessibility

The App template makes use of Simple Accessibility mode which resolves some issues with focus nesting and also makes screen readers work a lot more similarly on different platforms. further documentation can be found [here](https://platform.uno/docs/articles/features/working-with-accessibility.html#simpleaccessibility-mode) 

## Supported features

The way a screen reader will behave within the app is relatively simple. When first accessing a new page the focus will generally be "grabbed" by the topmost element of the page. After which the user can cycle through elements by swiping left/right on their device. The screen reader will then focus the next eligible element.

For an element to be focusable, it needs to have an accessibilityTrait. Currently in Uno, as listed [here](https://platform.uno/docs/articles/features/working-with-accessibility.html#known-issues), only the following elements support accessibility by default:
- Button
	- the screen reader will read the state of the button, it's text content and it's type.
- CheckBox
	- the screen reader will read the state of the checkbox, it's text content and it's type.
- FlipViewItem
	- the screen reader will read the text / content of each element contained in the item.
- ListViewItem
	- the screen reader will read the text / content of each element contained in the item.
- HyperlinkButton
	- the screen reader will read the state of the button, it's text content and it's type 
- Image
	- the screen reader will focus the image but read nothing by default.
- PasswordBox
	- on iOS, the screen reader will read the placeholder text only
	- on Android, focus on textboxes works a little differently where the focus will first take the whole box and read the placeholder text and cycling to the next element will focus the inner textBox and read it's content
- RadioButton
	- the screen reader will state wether the button is selected and then read it's content.
- TextBlock
	- the screen reader will read the Text property of the Textblock (or of the Runs contained within)
- TextBox
	- on iOS, the screen reader will read the placeholder text only
	- on Android, focus on textboxes works a little differently where the focus will first take the whole box and read the placeholder text and cycling to the next element will focus the inner textBox and read it's content
- ToggleButton
	- the screen reader will state wether the button is checked or not and then read the content.
- ToggleSwitch
	- the screen reader will state wether the button is checked or not and then read the content.


### Overriding text
If the default behavior of a control doesn't fit the requirements, it's possible to change the text that will be read when an element is focused through the use of the `AutomationProperties.Name` property of an object. this will only change the "Content" part of what the screen reader will read, for instance with a ToggleButton, it would still read the state of the button and then read the string contained in `AutomationProperties.Name` instead of the content.

## Important notes


### Commands
To make sure that commands work correctly with a screen reader turned on, it's important that the command is attached to the control that will be focused by the screen reader or else the command will not work.

#### Not working code
```
<utu:TabBarItem Command="{Binding ShowSettingsSection}">
	<c:BottomTabBarButton IconStyle="{StaticResource SettingsPathControlStyle}"
						  Foreground="{StaticResource MaterialOnPrimaryMediumBrush}"
						  Content="Profile"
						  x:Uid="Menu_Settings" />
</utu:TabBarItem>
```

#### Working code
```
<utu:TabBarItem>
	<c:BottomTabBarButton IconStyle="{StaticResource SettingsPathControlStyle}"
						  Command="{Binding ShowSettingsSection}"
						  Foreground="{StaticResource MaterialOnPrimaryMediumBrush}"
						  Content="Profile"
						  x:Uid="Menu_Settings" />
</utu:TabBarItem>
```

#### Custom control alternative
Alternatively, if it's important to keep the command on another control and that it is not focusable, a solution to this is to create a custom version of that control and override a method to make it so that it returns an AutomationPeer, that way, it will be recognised by the screen reader.

In this example it would be done like this:
First by creating the new control that extends TabBarItem
```csharp
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Automation.Peers;
using Uno.Toolkit.UI;

namespace ApplicationTemplate.Views.Controls
{
	public partial class ButtonTabBarItem : TabBarItem
	{
		protected override AutomationPeer OnCreateAutomationPeer()
		{
			return new ButtonTabBarItemAutomationPeer(this);
		}
	}
}
```

Then by creating the new AutomationPeer for that control:
```csharp
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Automation.Peers;

namespace ApplicationTemplate.Views.Controls
{
	public class ButtonTabBarItemAutomationPeer : FrameworkElementAutomationPeer
	{
		public ButtonTabBarItemAutomationPeer(ButtonTabBarItem owner) : base(owner)
		{
		}

		protected override AutomationControlType GetAutomationControlTypeCore()
		{
			return AutomationControlType.Button;
		}
	}
}
```
Then by going in the xaml and changing the control used:
```
<c:ButtonTabBarItem Command="{Binding ShowHomeSection}">
	<c:BottomTabBarButton IconStyle="{StaticResource HomePathControlStyle}"
						  Foreground="{StaticResource MaterialOnPrimaryMediumBrush}"
						  Content="Jokes"
						  x:Uid="Menu_Home" />
</c:ButtonTabBarItem>
```