using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using Chinook.DynamicMvvm;
using MessageDialogService;

namespace ApplicationTemplate.Presentation
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2201:Do not raise reserved exception types", Justification = "Those are diagnostics exception to test how the app reacts to exceptions.")]
	public class ExceptionsDiagnosticsViewModel : ViewModel
	{
		public IDynamicCommand TestErrorInCommand => this.GetCommand(() =>
		{
			throw new Exception("This is a test of an exception in a command. Please ignore.");
		});

		public IDynamicCommand TestErrorInTaskScheduler => this.GetCommand(() =>
		{
			// This will be handled by <see cref="TaskScheduler.UnobservedTaskException" />
			var _ = Task.Run(() => throw new Exception("This is a test of an exception in the TaskScheduler. Please ignore."));

			// Wait until the task had a chance to throw without awaiting it.
			// Unobserved tasks exceptions occur only when the tasks are collected by the GC.
			Thread.Sleep(100);
			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
			GC.WaitForPendingFinalizers();
		});

		public IDynamicCommand TestErrorInCoreDispatcher => this.GetCommandFromTask(async ct =>
		{
			await this.GetService<IDispatcherScheduler>().Run(ct2 => throw new Exception("This is a test of an exception in the CoreDispatcher. Please ignore."), ct);
		});

		public IDynamicCommand TestErrorInThreadPool => this.GetCommandFromTask(async ct =>
		{
			var confirmation = await this.GetService<IMessageDialogService>().ShowMessage(ct, mb => mb
				.Title("Diagnostics")
				.Content("This should crash your application. Make sure your analytics provider receives a crash log.")
				.CancelCommand()
				.Command(MessageDialogResult.Accept, label: "Crash")
			);

			if (confirmation != MessageDialogResult.Accept)
			{
				return;
			}

			// This will be handled by <see cref="AppDomain.CurrentDomain.UnhandledException" />
			var _ = ThreadPool.QueueUserWorkItem(__ => throw new Exception("This is a test of an exception in the ThreadPool. Please ignore."));
		});

		public IDynamicCommand TestErrorInMainThread => this.GetCommandFromTask(async ct =>
		{
			await this.GetService<IDiagnosticsService>().TestExceptionFromMainThread(ct);
		});
	}
}
