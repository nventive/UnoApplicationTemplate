using System;
using System.Collections.Generic;
using System.Text;
using ApplicationTemplate.Client;
using Chinook.DynamicMvvm;

namespace ApplicationTemplate.Presentation;

public class HttpTraceItemViewModel : ViewModel
{
	private readonly HttpTrace _trace;

	public HttpTraceItemViewModel(HttpTrace trace)
	{
		_trace = trace;

		TraceProperty = this.GetProperty(x => x.Trace);
	}

	public HttpTrace Trace
	{
		get => this.Get(initialValue: _trace);
		set => this.Set(value);
	}

	public IDynamicProperty<HttpTrace> TraceProperty { get; }

	public string MainLine => this.GetFromDynamicProperty(TraceProperty, GetMainLine);

	public string Icon => this.GetFromDynamicProperty(TraceProperty, GetIcon);

	private static string GetMainLine(HttpTrace trace)
	{
		var sb = new StringBuilder();
		AppendLongWithPadding(trace.SequenceId, 4, sb);
		sb.Append(' ');
		AppendWithPadding(trace.HttpMethod, 7, sb);
		AppendWithPadding((int?)trace.StatusCode, 4, sb);

		return sb.ToString();

		static void AppendLongWithPadding(long number, int maxLength, StringBuilder sb)
		{
			var padding = maxLength - GetLength(number);
			if (padding > 0)
			{
				sb.Append(' ', padding);
			}

			sb.Append(number);
		}

		static void AppendWithPadding(object value, int maxLength, StringBuilder sb)
		{
			var stringValue = value?.ToString() ?? string.Empty;
			sb.Append(stringValue);

			var padding = maxLength - stringValue.Length;
			if (padding > 0)
			{
				sb.Append(' ', padding);
			}
		}

		static int GetLength(long i)
		{
			if (i == 0)
			{
				return 1;
			}
			return (int)Math.Floor(Math.Log10(i)) + 1;
		}
	}

	private static string GetIcon(HttpTrace trace)
	{
		return trace.Status switch
		{
			HttpTraceStatus.Pending => "⏳",
			HttpTraceStatus.Received => "✅",
			HttpTraceStatus.Failed => "❌",
			HttpTraceStatus.Cancelled => "🚫",
			_ => throw new ArgumentOutOfRangeException(nameof(trace), trace, null)
		};
	}
}
