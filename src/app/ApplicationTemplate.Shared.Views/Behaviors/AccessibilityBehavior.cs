#if __WINDOWS__ || __ANDROID__ || __IOS__
using ApplicationTemplate.Views.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Uno.Toolkit.UI;

namespace ApplicationTemplate.Views.Behaviors;

public sealed class AccessibilityBehavior
{
	public static readonly DependencyProperty IsAccessibilityDisabledProperty =
		DependencyProperty.RegisterAttached("IsAccessibilityDisabled", typeof(bool), typeof(AccessibilityBehavior), new PropertyMetadata(default(bool), OnIsAccessibilityDisabledChanged));

	public static bool GetIsAccessibilityDisabled(DependencyObject obj)
	{
		return (bool)obj?.GetValue(IsAccessibilityDisabledProperty);
	}

	public static void SetIsAccessibilityDisabled(DependencyObject obj, bool value)
	{
		obj?.SetValue(IsAccessibilityDisabledProperty, value);
	}

	private static void SetInnerAccessibilityView(FrameworkElementAutomationPeer automationPeer, AccessibilityView accessibilityView)
	{
		AutomationProperties.SetAccessibilityView(automationPeer.Owner, accessibilityView);

		var automationPeerChildren = automationPeer.GetChildren();
		foreach (var automationPeerChild in automationPeerChildren)
		{
			SetInnerAccessibilityView(automationPeerChild as FrameworkElementAutomationPeer, accessibilityView);
		}
	}

	private static void OnIsAccessibilityDisabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		var isModalActive = (bool)e.NewValue;
		var accessibilityView = isModalActive ? AccessibilityView.Raw : AccessibilityView.Content;

		var children = d.GetChildren();

		foreach (var child in children)
		{
			var frameworkElement = child as FrameworkElement;
#if __ANDROID__
				frameworkElement.ImportantForAccessibility = isModalActive
					? Android.Views.ImportantForAccessibility.NoHideDescendants
					: Android.Views.ImportantForAccessibility.Yes;
#else
			var automationPeer = FrameworkElementAutomationPeer.CreatePeerForElement(frameworkElement);
			SetInnerAccessibilityView(automationPeer as FrameworkElementAutomationPeer, accessibilityView);
#endif
		}
	}
}
#endif
