using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ApplicationTemplate.Views;
using Microsoft.UI.Dispatching;
using Xamarin.Essentials.Interfaces;

namespace ApplicationTemplate;

/// <summary>
/// Invokes calls to an IBrowser on the dispatcher
/// </summary>
public class DispatcherBrowserDecorator : IBrowser
{
	private readonly IBrowser _innerBrowser;
	private readonly DispatcherQueue _dispatcher;

	public DispatcherBrowserDecorator(IBrowser browser, DispatcherQueue dispatcher)
	{
		_innerBrowser = browser;
		_dispatcher = dispatcher;
	}

	public async Task OpenAsync(string uri)
	{
		await DispatcherRunTaskAsync(DispatcherQueuePriority.Normal, () => _innerBrowser.OpenAsync(new Uri(uri)));
	}

	public async Task OpenAsync(string uri, Microsoft.Maui.ApplicationModel.BrowserLaunchMode launchMode)
	{
		await DispatcherRunTaskAsync(DispatcherQueuePriority.Normal, () => _innerBrowser.OpenAsync(new Uri(uri), launchMode));
	}

	public async Task OpenAsync(string uri, Microsoft.Maui.ApplicationModel.BrowserLaunchOptions options)
	{
		await DispatcherRunTaskAsync(DispatcherQueuePriority.Normal, () => _innerBrowser.OpenAsync(new Uri(uri), options));
	}

	public async Task OpenAsync(Uri uri)
	{
		await DispatcherRunTaskAsync(DispatcherQueuePriority.Normal, () => _innerBrowser.OpenAsync(uri));
	}

	public async Task OpenAsync(Uri uri, Microsoft.Maui.ApplicationModel.BrowserLaunchMode launchMode)
	{
		await DispatcherRunTaskAsync(DispatcherQueuePriority.Normal, () => _innerBrowser.OpenAsync(uri, launchMode));
	}

	public async Task<bool> OpenAsync(Uri uri, Microsoft.Maui.ApplicationModel.BrowserLaunchOptions options)
	{
		return await DispatcherRunTaskAsync(DispatcherQueuePriority.Normal, async () => await _innerBrowser.OpenAsync(uri, options));
	}

	/// <summary>
	/// This method allows for executing an async Task with result on the CoreDispatcher.
	/// </summary>
	private async Task<TResult> DispatcherRunTaskAsync<TResult>(DispatcherQueuePriority priority, Func<Task<TResult>> asyncFunc)
	{
		var completion = new TaskCompletionSource<TResult>();
		await _dispatcher.EnqueueAsync(RunActionUI, priority);
		return await completion.Task;

		async void RunActionUI()
		{
			try
			{
				var result = await asyncFunc();
				completion.SetResult(result);
			}
			catch (Exception exception)
			{
				completion.SetException(exception);
			}
		}
	}

	/// <summary>
	/// This method allows for executing an async Task without result on the CoreDispatcher.
	/// </summary>
	private async Task DispatcherRunTaskAsync(DispatcherQueuePriority priority, Func<Task> asyncFunc)
	{
		var completion = new TaskCompletionSource<Unit>();
		await _dispatcher.EnqueueAsync(RunActionUI, priority);
		await completion.Task;

		async void RunActionUI()
		{
			try
			{
				await asyncFunc();
				completion.SetResult(Unit.Default);
			}
			catch (Exception exception)
			{
				completion.SetException(exception);
			}
		}
	}
}
