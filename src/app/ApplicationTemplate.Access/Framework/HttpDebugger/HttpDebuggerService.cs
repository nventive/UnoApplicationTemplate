using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using Uno.Disposables;

namespace ApplicationTemplate.DataAccess;

public sealed class HttpDebuggerService : IHttpDebuggerService, IDisposable
{
	// Static empty values to avoid creating duplicated instances.
	private static readonly IEnumerable<KeyValuePair<string, IEnumerable<string>>> _emptyHeaders = Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>();
	private static readonly Task<string> _emptyContentTask = Task.FromResult<string>(null);

	/// <summary>
	/// Mutex to use when changing <see cref="_traceIds"/> and <see cref="_traces"/>.
	/// </summary>
	private readonly object _collectionMutex = new();

	/// <summary>
	/// The observable source containing only the ids.
	/// </summary>
	private readonly SourceList<long> _traceIds = new();

	/// <summary>
	/// The dictionary mapping ids to <see cref="HttpTrace"/> instances.
	/// </summary>
	private readonly Dictionary<long, HttpTrace> _traces = new();

	/// <remarks>
	/// It starts at -1 so that the first produced value is 0.
	/// </remarks>
	private long _idSource = -1;

	public IObservableList<long> Traces => _traceIds;

	public event EventHandler<TraceUpdatedEventArgs> TraceUpdated;

	public async Task<HttpTrace> StartTrace(HttpRequestMessage request)
	{
		var trace = new HttpTrace
		{
			SequenceId = Interlocked.Increment(ref _idSource),
			Status = HttpTraceStatus.Pending,
			HttpMethod = request.Method,
			RequestUri = request.RequestUri,
			RequestVersion = request.Version,
			RequestHeaders = request.Headers,
			RequestContentHeaders = request.Content?.Headers ?? _emptyHeaders,
			RequestContent = await (request.Content?.ReadAsStringAsync() ?? _emptyContentTask),
		};

		lock (_collectionMutex)
		{
			_traces.Add(trace.SequenceId, trace);
			_traceIds.Add(trace.SequenceId);
		}

		return trace;
	}

	public HttpTrace GetTrace(long traceId)
	{
		return _traces[traceId];
	}

	public void UpdateTrace(HttpTrace trace)
	{
		lock (_collectionMutex)
		{
			if (!_traces.ContainsKey(trace.SequenceId))
			{
				// Don't raise the updated event if the trace was removed.
				return;
			}

			_traces[trace.SequenceId] = trace;
		}
		TraceUpdated?.Invoke(this, new TraceUpdatedEventArgs { UpdatedTrace = trace });
	}

	public void RemoveTraces(IEnumerable<long> traceIds)
	{
		lock (_collectionMutex)
		{
			foreach (var traceId in traceIds)
			{
				_traces.Remove(traceId);
			}
			_traceIds.RemoveMany(traceIds);
		}
	}

	public void ClearTraces()
	{
		lock (_collectionMutex)
		{
			_traces.Clear();
			_traceIds.Clear();
		}
	}

	public void Dispose()
	{
		_traceIds.Dispose();
	}
}
