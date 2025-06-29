// src/app/ApplicationTemplate.Shared.Views/PlatformServices/Toast/ToastService.iOS.cs
#if __IOS__
using System;
using CPS.DataAccess.PlatformServices;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace CPS.DataAccess.PlatformServices;

public class ToastService : IToastService
{
	private readonly DispatcherQueue _dispatcherQueue;
	private readonly Panel _container;

	public ToastService(DispatcherQueue dispatcherQueue, Panel container)
	{
		_dispatcherQueue = dispatcherQueue;
		_container = container;
	}

	public void ShowNotification(string message, ToastDuration duration = ToastDuration.Short)
	{
		_dispatcherQueue.TryEnqueue(() =>
		{
			var toast = new Border
			{
				Background = new SolidColorBrush(Microsoft.UI.Colors.Black) { Opacity = 0.8 },
				CornerRadius = new CornerRadius(8),
				Padding = new Thickness(16, 8),
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Bottom,
				Margin = new Thickness(0, 0, 0, 100),
				Child = new TextBlock
				{
					Text = message,
					Foreground = new SolidColorBrush(Microsoft.UI.Colors.White),
					TextAlignment = TextAlignment.Center
				}
			};

			_container.Children.Add(toast);

			var timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds((int)duration)
			};

			timer.Tick += (s, e) =>
			{
				timer.Stop();
				_container.Children.Remove(toast);
			};

			timer.Start();
		});
	}
}
#endif
