using System;
using System.Collections.Immutable;

namespace ApplicationTemplate.Client
{
	public record HttpRequestContent
	{
		public HttpRequestContent(string method, string uri, string body, ImmutableDictionary<string, string> headers, DateTimeOffset timeStart)
		{
			Method = method;
			Uri = uri;
			Body = body;
			Headers = headers;
			TimeStart = timeStart;
		}

		public string Method { get; }

		public string Uri { get; }

		public string Body { get; }

		public ImmutableDictionary<string, string> Headers { get; }

		public DateTimeOffset TimeStart { get; }
	}
}
