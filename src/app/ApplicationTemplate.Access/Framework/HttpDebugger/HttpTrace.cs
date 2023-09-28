using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace ApplicationTemplate.DataAccess;

/// <summary>
/// Represents a trace of an Http request and response.
/// </summary>
public record HttpTrace
{
	public long SequenceId { get; init; }

	public HttpTraceStatus Status { get; init; }

	public TimeSpan? ElapsedTime { get; init; }

	public Uri RequestUri { get; init; }

	public HttpMethod HttpMethod { get; init; }

	public Version RequestVersion { get; init; }

	public string RequestContent { get; init; }

	public IEnumerable<KeyValuePair<string, IEnumerable<string>>> RequestHeaders { get; init; }

	public IEnumerable<KeyValuePair<string, IEnumerable<string>>> RequestContentHeaders { get; init; }

	public Version ResponseVersion { get; init; }

	public HttpStatusCode? StatusCode { get; init; }

	public string ReasonPhrase { get; init; }

	public string ResponseContent { get; init; }

	public IEnumerable<KeyValuePair<string, IEnumerable<string>>> ResponseHeaders { get; init; }

	public IEnumerable<KeyValuePair<string, IEnumerable<string>>> ResponseContentHeaders { get; init; }

	public Exception Exception { get; init; }
}

/// <summary>
/// Specifies the status of an <see cref="HttpTrace"/>.
/// All traces should start as <see cref="Pending"/> and enventually update to something else.
/// </summary>
public enum HttpTraceStatus
{
	/// <summary>
	/// Status when the request was sent and is pending a response.
	/// </summary>
	Pending,

	/// <summary>
	/// Status when an http response was received (including http error codes).
	/// </summary>
	Received,

	/// <summary>
	/// Status when an error happen while getting the response.
	/// This will happen when there is no internet connection or the server fails to give back a response.
	/// </summary>
	Failed,

	/// <summary>
	/// Status when the request was cancelled.
	/// </summary>
	Cancelled
}
