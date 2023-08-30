using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Security;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading;
using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Xml;
using ApplicationTemplate.Views;
using Chinook.SectionsNavigation;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Uno.Extensions;
using Windows.Graphics.Display;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Numerics;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace ApplicationTemplate;

public sealed partial class App : Application
{
	public App()
	{
		Instance = this;

		Startup = new Startup();
		Startup.PreInitialize();

		InitializeComponent();

		ConfigureOrientation();

		//-:cnd:noEmit
#if __MOBILE__
		LeavingBackground += OnLeavingBackground;
		Resuming += OnResuming;
		Suspending += OnSuspending;
#endif
		//+:cnd:noEmit

		this.Log().LogTrace(typeof(object).ToString());
		this.Log().LogTrace(typeof(string).ToString());
		this.Log().LogTrace(typeof(Array).ToString());
		this.Log().LogTrace(typeof(List<>).ToString());
		this.Log().LogTrace(typeof(Dictionary<,>).ToString());
		this.Log().LogTrace(typeof(HashSet<>).ToString());
		this.Log().LogTrace(typeof(Queue<>).ToString());
		this.Log().LogTrace(typeof(Stack<>).ToString());
		this.Log().LogTrace(typeof(Tuple<,>).ToString());
		this.Log().LogTrace(typeof(Enum).ToString());
		this.Log().LogTrace(typeof(Nullable<>).ToString());
		this.Log().LogTrace(typeof(DateTime).ToString());
		this.Log().LogTrace(typeof(TimeSpan).ToString());
		this.Log().LogTrace(typeof(Guid).ToString());
		this.Log().LogTrace(typeof(Exception).ToString());
		this.Log().LogTrace(typeof(EventHandler<>).ToString());
		this.Log().LogTrace(typeof(Thread).ToString());
		this.Log().LogTrace(typeof(Task).ToString());
		this.Log().LogTrace(typeof(Task<>).ToString());
		this.Log().LogTrace(typeof(CancellationToken).ToString());
		this.Log().LogTrace(typeof(HttpClient).ToString());
		this.Log().LogTrace(typeof(FileStream).ToString());
		this.Log().LogTrace(typeof(MemoryStream).ToString());
		this.Log().LogTrace(typeof(StreamReader).ToString());
		this.Log().LogTrace(typeof(StreamWriter).ToString());
		this.Log().LogTrace(typeof(XmlDocument).ToString());
		this.Log().LogTrace(typeof(XDocument).ToString());
		this.Log().LogTrace(typeof(XmlElement).ToString());
		this.Log().LogTrace(typeof(XElement).ToString());
		this.Log().LogTrace(typeof(StringBuilder).ToString());
		this.Log().LogTrace(typeof(Regex).ToString());
		this.Log().LogTrace(typeof(Uri).ToString());
		this.Log().LogTrace(typeof(Convert).ToString());
		this.Log().LogTrace(typeof(Math).ToString());
		this.Log().LogTrace(typeof(Random).ToString());
		this.Log().LogTrace(typeof(Console).ToString());
		this.Log().LogTrace(typeof(Environment).ToString());
		this.Log().LogTrace(typeof(File).ToString());
		this.Log().LogTrace(typeof(Directory).ToString());
		this.Log().LogTrace(typeof(Path).ToString());
		this.Log().LogTrace(typeof(FileInfo).ToString());
		this.Log().LogTrace(typeof(DirectoryInfo).ToString());
		this.Log().LogTrace(typeof(Stream).ToString());
		this.Log().LogTrace(typeof(MemoryStream).ToString());
		this.Log().LogTrace(typeof(FileStream).ToString());
		this.Log().LogTrace(typeof(BinaryReader).ToString());
		this.Log().LogTrace(typeof(BinaryWriter).ToString());
		this.Log().LogTrace(typeof(Encoding).ToString());
		this.Log().LogTrace(typeof(StringReader).ToString());
		this.Log().LogTrace(typeof(StringWriter).ToString());
		this.Log().LogTrace(typeof(UriBuilder).ToString());
		this.Log().LogTrace(typeof(Version).ToString());
		this.Log().LogTrace(typeof(UriFormatException).ToString());
		this.Log().LogTrace(typeof(AggregateException).ToString());
		this.Log().LogTrace(typeof(ArgumentNullException).ToString());
		this.Log().LogTrace(typeof(ArgumentException).ToString());
		this.Log().LogTrace(typeof(InvalidOperationException).ToString());
		this.Log().LogTrace(typeof(NotSupportedException).ToString());
		this.Log().LogTrace(typeof(FormatException).ToString());
		this.Log().LogTrace(typeof(IndexOutOfRangeException).ToString());
		this.Log().LogTrace(typeof(TimeoutException).ToString());
		this.Log().LogTrace(typeof(OperationCanceledException).ToString());
		this.Log().LogTrace(typeof(EventHandler<>).ToString());
		this.Log().LogTrace(typeof(ThreadStart).ToString());
		this.Log().LogTrace(typeof(TaskFactory).ToString());
		this.Log().LogTrace(typeof(TaskCompletionSource<>).ToString());
		this.Log().LogTrace(typeof(HttpResponseMessage).ToString());
		this.Log().LogTrace(typeof(HttpRequestMessage).ToString());
		this.Log().LogTrace(typeof(HttpContent).ToString());
		this.Log().LogTrace(typeof(HttpRequestException).ToString());
		this.Log().LogTrace(typeof(WebException).ToString());
		this.Log().LogTrace(typeof(WebResponse).ToString());
		this.Log().LogTrace(typeof(WebRequest).ToString());
		this.Log().LogTrace(typeof(XmlReader).ToString());
		this.Log().LogTrace(typeof(XmlWriter).ToString());
		this.Log().LogTrace(typeof(XmlElement).ToString());
		this.Log().LogTrace(typeof(XmlAttribute).ToString());
		this.Log().LogTrace(typeof(XmlDocumentFragment).ToString());
		this.Log().LogTrace(typeof(XmlNamespaceManager).ToString());
		this.Log().LogTrace(typeof(XPathNavigator).ToString());
		this.Log().LogTrace(typeof(XsltArgumentList).ToString());
		this.Log().LogTrace(typeof(XslCompiledTransform).ToString());
		this.Log().LogTrace(typeof(HttpClientHandler).ToString());
		this.Log().LogTrace(typeof(HttpStatusCode).ToString());
		this.Log().LogTrace(typeof(HttpRequestHeaders).ToString());
		this.Log().LogTrace(typeof(HttpResponseHeaders).ToString());
		this.Log().LogTrace(typeof(CookieContainer).ToString());
		this.Log().LogTrace(typeof(WebRequestMethods).ToString());
		this.Log().LogTrace(typeof(PathTooLongException).ToString());
		this.Log().LogTrace(typeof(StreamReader).ToString());
		this.Log().LogTrace(typeof(StreamWriter).ToString());
		this.Log().LogTrace(typeof(SecurityException).ToString());
		this.Log().LogTrace(typeof(SemaphoreSlim).ToString());
		this.Log().LogTrace(typeof(Stack<>).ToString());
		this.Log().LogTrace(typeof(SortedDictionary<,>).ToString());
		this.Log().LogTrace(typeof(SortedSet<>).ToString());
		this.Log().LogTrace(typeof(SpinLock).ToString());
		this.Log().LogTrace(typeof(StringBuilder).ToString());
		this.Log().LogTrace(typeof(Stream).ToString());
		this.Log().LogTrace(typeof(StreamReader).ToString());
		this.Log().LogTrace(typeof(StreamWriter).ToString());
		this.Log().LogTrace(typeof(StringComparer).ToString());
		this.Log().LogTrace(typeof(StringReader).ToString());
		this.Log().LogTrace(typeof(StringWriter).ToString());
		this.Log().LogTrace(typeof(StringSplitOptions).ToString());
		this.Log().LogTrace(typeof(StructLayoutAttribute).ToString());
		this.Log().LogTrace(typeof(TaskCanceledException).ToString());
		this.Log().LogTrace(typeof(TaskContinuationOptions).ToString());
		this.Log().LogTrace(typeof(TaskFactory).ToString());
		this.Log().LogTrace(typeof(TaskScheduler).ToString());
		this.Log().LogTrace(typeof(TextReader).ToString());
		this.Log().LogTrace(typeof(TextWriter).ToString());
		this.Log().LogTrace(typeof(ThreadAbortException).ToString());
		this.Log().LogTrace(typeof(ThreadState).ToString());
		this.Log().LogTrace(typeof(ThreadStart).ToString());
		this.Log().LogTrace(typeof(TimeSpan).ToString());
		this.Log().LogTrace(typeof(TimeoutException).ToString());
		this.Log().LogTrace(typeof(Timer).ToString());
		this.Log().LogTrace(typeof(TimeZoneInfo).ToString());
		this.Log().LogTrace(typeof(Type).ToString());
		this.Log().LogTrace(typeof(TypedReference).ToString());
		this.Log().LogTrace(typeof(TypeInitializationException).ToString());
		this.Log().LogTrace(typeof(TypeLoadException).ToString());
		this.Log().LogTrace(typeof(TypeUnloadedException).ToString());
		this.Log().LogTrace(typeof(UInt16).ToString());
		this.Log().LogTrace(typeof(UInt32).ToString());
		this.Log().LogTrace(typeof(UInt64).ToString());
		this.Log().LogTrace(typeof(UnauthorizedAccessException).ToString());
		this.Log().LogTrace(typeof(Uri).ToString());
		this.Log().LogTrace(typeof(UriBuilder).ToString());
		this.Log().LogTrace(typeof(UriFormatException).ToString());
		this.Log().LogTrace(typeof(UriHostNameType).ToString());
		this.Log().LogTrace(typeof(UriKind).ToString());
		this.Log().LogTrace(typeof(UriPartial).ToString());
		this.Log().LogTrace(typeof(UriTypeConverter).ToString());
		this.Log().LogTrace(typeof(UriComponents).ToString());
		this.Log().LogTrace(typeof(ValueTuple).ToString());
		this.Log().LogTrace(typeof(Vector).ToString());
		this.Log().LogTrace(typeof(Version).ToString());
		this.Log().LogTrace(typeof(Volatile).ToString());
		this.Log().LogTrace(typeof(WeakReference).ToString());
		this.Log().LogTrace(typeof(WebException).ToString());
		this.Log().LogTrace(typeof(WebExceptionStatus).ToString());
		this.Log().LogTrace(typeof(WebHeaderCollection).ToString());
		this.Log().LogTrace(typeof(WebProxy).ToString());
		this.Log().LogTrace(typeof(WebResponse).ToString());
		this.Log().LogTrace(typeof(WebRequest).ToString());
		this.Log().LogTrace(typeof(WebUtility).ToString());
		this.Log().LogTrace(typeof(XmlAttribute).ToString());
		this.Log().LogTrace(typeof(XmlCDataSection).ToString());
		this.Log().LogTrace(typeof(XmlCharacterData).ToString());
		this.Log().LogTrace(typeof(XmlComment).ToString());
		this.Log().LogTrace(typeof(XmlDeclaration).ToString());
		this.Log().LogTrace(typeof(XmlDocument).ToString());
		this.Log().LogTrace(typeof(XmlDocumentFragment).ToString());
		this.Log().LogTrace(typeof(XmlDocumentType).ToString());
		this.Log().LogTrace(typeof(XmlElement).ToString());
		this.Log().LogTrace(typeof(XmlEntityReference).ToString());
		this.Log().LogTrace(typeof(XmlImplementation).ToString());
		this.Log().LogTrace(typeof(XmlLinkedNode).ToString());
		this.Log().LogTrace(typeof(XmlNamedNodeMap).ToString());
		this.Log().LogTrace(typeof(XmlNode).ToString());
		this.Log().LogTrace(typeof(XmlNodeList).ToString());
		this.Log().LogTrace(typeof(XmlNodeReader).ToString());
		this.Log().LogTrace(typeof(XmlProcessingInstruction).ToString());
		this.Log().LogTrace(typeof(XmlReader).ToString());
		this.Log().LogTrace(typeof(XmlSignificantWhitespace).ToString());
		this.Log().LogTrace(typeof(XmlText).ToString());
		this.Log().LogTrace(typeof(XmlTextWriter).ToString());
		this.Log().LogTrace(typeof(XmlWhitespace).ToString());
		this.Log().LogTrace(typeof(XmlWriter).ToString());
		this.Log().LogTrace(typeof(XPathDocument).ToString());
		this.Log().LogTrace(typeof(XPathExpression).ToString());
		this.Log().LogTrace(typeof(XPathNavigator).ToString());
		this.Log().LogTrace(typeof(XPathNodeIterator).ToString());
		this.Log().LogTrace(typeof(XPathNodeType).ToString());
		this.Log().LogTrace(typeof(XsltArgumentList).ToString());
		this.Log().LogTrace(typeof(XslCompiledTransform).ToString());
		this.Log().LogTrace(typeof(XsltSettings).ToString());
		this.Log().LogTrace(typeof(XslTransform).ToString());
		this.Log().LogTrace(typeof(ZipArchive).ToString());
		this.Log().LogTrace(typeof(ZipArchiveEntry).ToString());
		this.Log().LogTrace(typeof(ZipFile).ToString());
		this.Log().LogTrace(typeof(ZipFileExtensions).ToString());
		this.Log().LogTrace(typeof(TypeCode).ToString());
		this.Log().LogTrace(typeof(AppDomain).ToString());
		this.Log().LogTrace(typeof(Activator).ToString());
		this.Log().LogTrace(typeof(Attribute).ToString());
		this.Log().LogTrace(typeof(AttributeTargets).ToString());
		this.Log().LogTrace(typeof(AttributeUsageAttribute).ToString());
		this.Log().LogTrace(typeof(BitConverter).ToString());
		this.Log().LogTrace(typeof(Buffer).ToString());
		this.Log().LogTrace(typeof(Convert).ToString());
		this.Log().LogTrace(typeof(Console).ToString());
		this.Log().LogTrace(typeof(Convert).ToString());
		this.Log().LogTrace(typeof(Math).ToString());
		this.Log().LogTrace(typeof(Random).ToString());
		this.Log().LogTrace(typeof(Convert).ToString());
		this.Log().LogTrace(typeof(Math).ToString());
		this.Log().LogTrace(typeof(Random).ToString());
		this.Log().LogTrace(typeof(Guid).ToString());
		this.Log().LogTrace(typeof(Marshal).ToString());
		this.Log().LogTrace(typeof(RuntimeHelpers).ToString());
		this.Log().LogTrace(typeof(MathF).ToString());
		this.Log().LogTrace(typeof(Guid).ToString());
		this.Log().LogTrace(typeof(Marshal).ToString());
		this.Log().LogTrace(typeof(RuntimeHelpers).ToString());
		this.Log().LogTrace(typeof(MathF).ToString());
		this.Log().LogTrace(typeof(UIntPtr).ToString());
		this.Log().LogTrace(typeof(IntPtr).ToString());
		this.Log().LogTrace(typeof(UIntPtr).ToString());
		this.Log().LogTrace(typeof(IntPtr).ToString());
		this.Log().LogTrace(typeof(UIntPtr).ToString());
		this.Log().LogTrace(typeof(IntPtr).ToString());
		this.Log().LogTrace(typeof(ArraySegment<>).ToString());
		this.Log().LogTrace(typeof(Tuple<,>).ToString());
		this.Log().LogTrace(typeof(ValueTuple<,>).ToString());
		this.Log().LogTrace(typeof(HashSet<>).ToString());
		this.Log().LogTrace(typeof(HashSet<>).ToString());
		this.Log().LogTrace(typeof(TaskStatus).ToString());
		this.Log().LogTrace(typeof(TaskCreationOptions).ToString());
		this.Log().LogTrace(typeof(TaskContinuationOptions).ToString());
		this.Log().LogTrace(typeof(CancellationToken).ToString());
		this.Log().LogTrace(typeof(CancellationTokenSource).ToString());
		this.Log().LogTrace(typeof(IProgress<>).ToString());
		this.Log().LogTrace(typeof(IProgress<>).ToString());
		this.Log().LogTrace(typeof(IProgress<>).ToString());
		this.Log().LogTrace(typeof(TaskFactory).ToString());
		this.Log().LogTrace(typeof(TaskFactory<>).ToString());
		this.Log().LogTrace(typeof(TaskFactory).ToString());
		this.Log().LogTrace(typeof(TaskFactory<>).ToString());
		this.Log().LogTrace(typeof(Lazy<>).ToString());
		this.Log().LogTrace(typeof(LazyThreadSafetyMode).ToString());
		this.Log().LogTrace(typeof(Lazy<>).ToString());
		this.Log().LogTrace(typeof(LazyThreadSafetyMode).ToString());
		this.Log().LogTrace(typeof(BlockingCollection<>).ToString());
		this.Log().LogTrace(typeof(ConcurrentBag<>).ToString());
		this.Log().LogTrace(typeof(ConcurrentDictionary<,>).ToString());
		this.Log().LogTrace(typeof(ConcurrentQueue<>).ToString());
		this.Log().LogTrace(typeof(ConcurrentStack<>).ToString());
		this.Log().LogTrace(typeof(Interlocked).ToString());
		this.Log().LogTrace(typeof(Monitor).ToString());
		this.Log().LogTrace(typeof(Mutex).ToString());
		this.Log().LogTrace(typeof(ReaderWriterLock).ToString());
		this.Log().LogTrace(typeof(Semaphore).ToString());
		this.Log().LogTrace(typeof(SemaphoreSlim).ToString());
		this.Log().LogTrace(typeof(Barrier).ToString());
		this.Log().LogTrace(typeof(Thread).ToString());
		this.Log().LogTrace(typeof(ThreadStart).ToString());
		this.Log().LogTrace(typeof(ParameterizedThreadStart).ToString());
		this.Log().LogTrace(typeof(ThreadPriority).ToString());
		this.Log().LogTrace(typeof(ThreadState).ToString());
		this.Log().LogTrace(typeof(ManualResetEvent).ToString());
		this.Log().LogTrace(typeof(ManualResetEventSlim).ToString());
		this.Log().LogTrace(typeof(AutoResetEvent).ToString());
		this.Log().LogTrace(typeof(CountdownEvent).ToString());
		this.Log().LogTrace(typeof(CancellationToken).ToString());
		this.Log().LogTrace(typeof(CancellationTokenRegistration).ToString());
		this.Log().LogTrace(typeof(CancellationTokenSource).ToString());
		this.Log().LogTrace(typeof(Progress<>).ToString());
		this.Log().LogTrace(typeof(Progress<>).ToString());
		this.Log().LogTrace(typeof(Progress<>).ToString());
		this.Log().LogTrace(typeof(ConditionalWeakTable<,>).ToString());
		this.Log().LogTrace(typeof(RuntimeTypeHandle).ToString());
		this.Log().LogTrace(typeof(RuntimeMethodHandle).ToString());
		this.Log().LogTrace(typeof(RuntimeFieldHandle).ToString());
		this.Log().LogTrace(typeof(Delegate).ToString());
		this.Log().LogTrace(typeof(MulticastDelegate).ToString());
		this.Log().LogTrace(typeof(AppContext).ToString());
		this.Log().LogTrace(typeof(AppDomain).ToString());

	}

	public static App Instance { get; private set; }

	public static Startup Startup { get; private set; }

	public Shell Shell { get; private set; }

	public MultiFrame NavigationMultiFrame => Shell?.NavigationMultiFrame;

	public Window CurrentWindow { get; private set; }

	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		InitializeAndStart();
	}

	private void InitializeAndStart()
	{
//-:cnd:noEmit
#if __WINDOWS__
		CurrentWindow = new Window();
		CurrentWindow.Activate();
#else
		CurrentWindow = Microsoft.UI.Xaml.Window.Current;
#endif
//+:cnd:noEmit

		Shell = CurrentWindow.Content as Shell;

		var isFirstLaunch = Shell == null;

		if (isFirstLaunch)
		{
			ConfigureWindow();
			ConfigureStatusBar();

			Startup.Initialize(GetContentRootPath(), GetSettingsFolderPath(), LoggingConfiguration.ConfigureLogging);

			Startup.ShellActivity.Start();

			CurrentWindow.Content = Shell = new Shell();

			Startup.ShellActivity.Stop();
		}

//-:cnd:noEmit
#if __MOBILE__
		CurrentWindow.Activate();
#endif
//+:cnd:noEmit

		_ = Task.Run(() => Startup.Start());
	}

//-:cnd:noEmit
#if __MOBILE__
	/// <summary>
	/// This is where your app launches if you use custom schemes, Universal Links, or Android App Links.
	/// </summary>
	/// <param name="args"><see cref="Windows.ApplicationModel.Activation.IActivatedEventArgs"/>.</param>
	protected override void OnActivated(Windows.ApplicationModel.Activation.IActivatedEventArgs args)
	{
		InitializeAndStart();
	}

	private void OnLeavingBackground(object sender, Windows.ApplicationModel.LeavingBackgroundEventArgs e)
	{
		this.Log().LogInformation("Application is leaving background.");
	}

	private void OnResuming(object sender, object e)
	{
		this.Log().LogInformation("Application is resuming.");
	}

	private void OnSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
	{
		this.Log().LogInformation("Application is suspending.");
	}
#endif
//+:cnd:noEmit

	private static string GetContentRootPath()
	{
//-:cnd:noEmit
#if __WINDOWS__ || __MOBILE__
		return Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#else
		return string.Empty;
#endif
//+:cnd:noEmit
	}

	private static string GetSettingsFolderPath()
	{
//-:cnd:noEmit
#if __WINDOWS__
		return Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#else
		return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
#endif
//+:cnd:noEmit
	}

	private static void ConfigureOrientation()
	{
		DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
	}

	private static void ConfigureStatusBar()
	{
		var resources = Current.Resources;
		var statusBarHeight = 0d;

//-:cnd:noEmit
#if __MOBILE__
		Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ForegroundColor = Microsoft.UI.Colors.White;
		statusBarHeight = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().OccludedRect.Height;
#endif
//+:cnd:noEmit

		resources.Add("StatusBarDouble", statusBarHeight);
		resources.Add("StatusBarThickness", new Thickness(0, statusBarHeight, 0, 0));
		resources.Add("StatusBarGridLength", new GridLength(statusBarHeight, GridUnitType.Pixel));
	}

	private void ConfigureWindow()
	{
//-:cnd:noEmit
#if __WINDOWS__
		var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(CurrentWindow);
		var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
		var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
		appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 480, Height = 800 });

		// Sets a title bar icon and title.
		// Workaround. See https://github.com/microsoft/microsoft-ui-xaml/issues/6773 for more details.
		appWindow.SetIcon("Images\\TitleBarIcon.ico");
		appWindow.Title = "ApplicationTemplate";
#endif
//+:cnd:noEmit
	}
}
