using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using ApplicationTemplate.Client;
using Chinook.DynamicMvvm;
using DynamicData;
using Uno;
using Uno.Extensions;

namespace ApplicationTemplate.Presentation;

public partial class HttpDebuggerViewModel : ViewModel
{
	[Inject] private IHttpDebuggerService _httpDebuggerService;

	public HttpDebuggerViewModel()
	{
		_httpDebuggerService.TraceUpdated += OnTraceUpdated;
		AddDisposable(Disposable.Create(() => _httpDebuggerService.TraceUpdated -= OnTraceUpdated));

		AddDisposable(_httpDebuggerService
			.Traces
			.Connect()
			.Transform(sequenceId => this.GetChild(
				() => new HttpTraceItemViewModel(_httpDebuggerService.GetTrace(sequenceId)),
				name: GetChildName(sequenceId))
			)
			.ObserveOn(this.GetService<IDispatcherScheduler>())
			.Bind(out var traces)
			.DisposeMany()
			.Subscribe()
		);

		Traces = traces;
	}

	private void OnTraceUpdated(object sender, TraceUpdatedEventArgs e)
	{
		if (this.TryGetDisposable<HttpTraceItemViewModel>(GetChildName(e.UpdatedTrace.SequenceId), out var vm))
		{
			vm.Trace = e.UpdatedTrace;
		}
	}

	public ReadOnlyObservableCollection<HttpTraceItemViewModel> Traces { get; private set; }

	public int SelectedIndex
	{
		get => this.Get(initialValue: -1);
		set => this.Set(value);
	}

	public HttpTraceItemViewModel SelectedTrace => this.GetFromDynamicProperty(
		this.GetProperty(x => x.SelectedIndex),
		index => index switch
		{
			< 0 => null,
			>= 0 => Traces[index]
		}
	);

	public bool IsEnabled
	{
		get => this.GetFromOptionsMonitor<DiagnosticsOptions, bool>(o => o.IsHttpDebuggerEnabled);
		set => this.Set(value);
	}

	public bool HideRequestHeaders
	{
		get => this.GetFromOptionsMonitor<DiagnosticsOptions, bool>(o => o.HttpDebugger.HideRequestHeaders);
		set => this.Set(value);
	}

	public bool HideResponseHeaders
	{
		get => this.GetFromOptionsMonitor<DiagnosticsOptions, bool>(o => o.HttpDebugger.HideResponseHeaders);
		set => this.Set(value);
	}

	public bool FormatRequestContent
	{
		get => this.GetFromOptionsMonitor<DiagnosticsOptions, bool>(o => o.HttpDebugger.FormatRequestContent);
		set => this.Set(value);
	}

	public bool FormatResponseContent
	{
		get => this.GetFromOptionsMonitor<DiagnosticsOptions, bool>(o => o.HttpDebugger.FormatResponseContent);
		set => this.Set(value);
	}

	public string RequestDetails => this.GetFromObservable(GetAndObserveRequestDetails(), initialValue: null);

	private IObservable<string> GetAndObserveRequestDetails()
	{
		return Observable.CombineLatest(
			this.GetProperty(x => x.SelectedTrace).GetAndObserve()
				.Select(vm => vm?.TraceProperty.GetAndObserve() ?? Observable.Never<HttpTrace>().StartWith(default(HttpTrace)))
				.Switch(),
			this.GetProperty(x => x.HideRequestHeaders).GetAndObserve(),
			this.GetProperty(x => x.FormatRequestContent).GetAndObserve(),
			(trace, hideHeaders, formatContent) =>
			{
				if (trace is null)
				{
					return null;
				}

				return GetRequestDetails(trace, hideHeaders, formatContent);
			});
	}

	public string ResponseDetails => this.GetFromObservable(GetAndObserveResponseDetails(), initialValue: null);

	private IObservable<string> GetAndObserveResponseDetails()
	{
		return Observable.CombineLatest(
			this.GetProperty(x => x.SelectedTrace).GetAndObserve()
				.Select(vm => vm?.TraceProperty.GetAndObserve() ?? Observable.Never<HttpTrace>().StartWith(default(HttpTrace)))
				.Switch(),
			this.GetProperty(x => x.HideResponseHeaders).GetAndObserve(),
			this.GetProperty(x => x.FormatResponseContent).GetAndObserve(),
			(trace, hideHeaders, formatContent) =>
			{
				if (trace is null)
				{
					return null;
				}

				return GetResponseDetails(trace, hideHeaders, formatContent);
			});
	}

	public IDynamicCommand CloseDetails => this.GetCommand(() =>
	{
		SelectedIndex = -1;
	});

	public IDynamicCommand Clear => this.GetCommand(() =>
	{
		_httpDebuggerService.ClearTraces();
	});

	public IDynamicCommand NotifyNeedsRestart => this.GetNotifyNeedsRestartCommand();

	private static string GetChildName(long sequenceId)
	{
		return sequenceId.ToString(CultureInfo.InvariantCulture);
	}

	private static string GetRequestDetails(HttpTrace trace, bool hideHeaders, bool formatContent)
	{
		var sb = new StringBuilder();

		var line1 = $"{trace.HttpMethod} {trace.RequestUri} HTTP/{trace.RequestVersion}";
		sb.AppendLine(line1);

		if (!hideHeaders)
		{
			foreach (var kvp in trace.RequestHeaders)
			{
				foreach (var val in kvp.Value)
				{
					var header = $"{kvp.Key}: {val}";
					sb.AppendLine(header);
				}
			}

			foreach (var kvp in trace.RequestContentHeaders)
			{
				foreach (var val in kvp.Value)
				{
					var header = $"{kvp.Key}: {val}";
					sb.AppendLine(header);
				}
			}
		}

		sb.AppendLine();

		var body = TryFormatContent(trace.RequestContent, formatContent);

		if (!string.IsNullOrWhiteSpace(body))
		{
			sb.AppendLine(body);
		}

		return sb.ToString();
	}

	private static string GetResponseDetails(HttpTrace trace, bool hideHeaders, bool formatContent)
	{
		if (trace.Status != HttpTraceStatus.Received)
		{
			return $"No response received.{Environment.NewLine}{Environment.NewLine}{trace.Exception}";
		}

		var sb = new StringBuilder();

		var statusCode = (int)trace.StatusCode;
		var line1 = $"HTTP/{trace.ResponseVersion} {statusCode} {trace.ReasonPhrase}";
		sb.AppendLine(line1);

		if (!hideHeaders)
		{
			foreach (var kvp in trace.ResponseHeaders)
			{
				foreach (var val in kvp.Value)
				{
					var header = $"{kvp.Key}: {val}";
					sb.AppendLine(header);
				}
			}

			foreach (var kvp in trace.ResponseContentHeaders)
			{
				foreach (var val in kvp.Value)
				{
					var header = $"{kvp.Key}: {val}";
					sb.AppendLine(header);
				}
			}
		}
		sb.AppendLine();

		var body = TryFormatContent(trace.ResponseContent, formatContent);

		if (!string.IsNullOrWhiteSpace(body))
		{
			sb.AppendLine(body);
		}

		return sb.ToString();
	}

	private static string TryFormatContent(string content, bool indentJson)
	{
		if (content.IsNullOrWhiteSpace())
		{
			return content;
		}

		if (indentJson)
		{
			var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(content));
			if (JsonDocument.TryParseValue(ref reader, out var jsonDoc))
			{
				return JsonSerializer.Serialize(jsonDoc, new JsonSerializerOptions { WriteIndented = true });
			}
		}

		return content;
	}
}
