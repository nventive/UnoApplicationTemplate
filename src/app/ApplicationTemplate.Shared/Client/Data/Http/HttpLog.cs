using System;

namespace ApplicationTemplate.Client
{
	public record HttpLog
	{
		public HttpLog(HttpRequestContent request, HttpResponseContent response = null, ProcessingStatus status = ProcessingStatus.Sent)
		{
			Request = request;
			Response = response;
			Status = status;
		}

		public Guid Id { get; } = Guid.NewGuid();

		public HttpRequestContent Request { get; }

		public HttpResponseContent Response { get; }

		public ProcessingStatus Status { get; }

		/// <summary>
		/// Used to know the the processing status of the HttpRequest.
		/// </summary>
		public enum ProcessingStatus
		{
			/// <summary>
			/// Describes the request as being sent.
			/// </summary>
			Sent,
			/// <summary>
			/// Describes the response as being received.
			/// </summary>
			Received,
			/// <summary>
			/// Describes an error in getting the response. This will happen when there is no internet connection or the server fails to give back a response.
			/// </summary>
			Failed,
			/// <summary>
			/// Describes a request which is cancelled.
			/// </summary>
			Cancelled
		}
	}
}

