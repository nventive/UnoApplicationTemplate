using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DynamicData;

namespace ApplicationTemplate.DataAccess;

/// <summary>
/// This service handles http traces for debugging purposes.
/// </summary>
public interface IHttpDebuggerService
{
	/// <summary>
	/// Creates a new trace from an <see cref="HttpRequestMessage"/>.
	/// </summary>
	/// <param name="request">The <see cref="HttpRequestMessage"/>.</param>
	/// <returns>The newly created <see cref="HttpTrace"/>.</returns>
	Task<HttpTrace> StartTrace(HttpRequestMessage request);

	/// <summary>
	/// Updates an existing trace.
	/// Matching is done using <see cref="HttpTrace.SequenceId"/>.
	/// </summary>
	/// <param name="trace">The new value.</param>
	void UpdateTrace(HttpTrace trace);

	/// <summary>
	/// Removes any combination of existing traces.
	/// </summary>
	/// <param name="traceIds">The enumerable of trace ids to remove.</param>
	void RemoveTraces(IEnumerable<long> traceIds);

	/// <summary>
	/// Removes all existing traces.
	/// </summary>
	void ClearTraces();

	/// <summary>
	/// Gets the <see cref="HttpTrace"/> associated with the <paramref name="traceId"/>.
	/// </summary>
	/// <param name="traceId">The sequence id of the desired trace.</param>
	/// <returns>The <see cref="HttpTrace"/> associated with the id.</returns>
	HttpTrace GetTrace(long traceId);

	/// <summary>
	/// Gets an observable list of trace ids.
	/// </summary>
	IObservableList<long> Traces { get; }

	/// <summary>
	/// Occurs when a trace is updated via <see cref="UpdateTrace(HttpTrace)"/>.
	/// </summary>
	event EventHandler<TraceUpdatedEventArgs> TraceUpdated;
}

/// <summary>
/// The <see cref="EventArgs"/> for the <see cref="IHttpDebuggerService.TraceUpdated"/> event.
/// </summary>
public class TraceUpdatedEventArgs : EventArgs
{
	/// <summary>
	/// Gets the <see cref="HttpTrace"/> that was updated via <see cref="IHttpDebuggerService.UpdateTrace(HttpTrace)"/>.
	/// </summary>
	public HttpTrace UpdatedTrace { get; init; }
}
