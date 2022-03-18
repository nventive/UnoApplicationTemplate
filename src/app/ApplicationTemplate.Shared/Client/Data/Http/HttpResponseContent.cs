using System;
using System.Collections.Immutable;
using System.Net;

namespace ApplicationTemplate.Client
{
	public record HttpResponseContent
	{
		public HttpResponseContent(HttpStatusCode statusCode, string body, ImmutableDictionary<string, string> headers, DateTimeOffset timeReceived)
		{
			StatusCode = statusCode;
			Body = body;
			Headers = headers;
			TimeReceived = timeReceived;
		}

		public HttpStatusCode StatusCode { get; }

		public string Body { get; }

		public ImmutableDictionary<string, string> Headers { get; }

		public DateTimeOffset TimeReceived { get; }
	}
}
