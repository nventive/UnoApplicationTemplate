using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using CommonServiceLocator;

namespace ApplicationTemplate.Client
{
	public class HttpTracerHandler : DelegatingHandler
	{
		private IHttpTracingService _tracingService;

		public HttpTracerHandler(IHttpTracingService tracingService)
		{
			_tracingService = tracingService;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var requestContents = await ExtractHttpRequestContents(request).ConfigureAwait(false);
			var log = new HttpLog(requestContents);
			var id = _tracingService?.AddLog(log) ?? Guid.Empty;

			try
			{
				var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

				var responseContents = await ExtractHttpResponseContents(response).ConfigureAwait(false);

				_tracingService?.UpdateLog(id, new HttpLog(
					request: requestContents,
					response: responseContents,
					HttpLog.ProcessingStatus.Received
				));

				return response;
			}
			catch (Exception)
			{
				_tracingService?.UpdateLog(id, new HttpLog(
					requestContents,
					status: HttpLog.ProcessingStatus.Failed
				));
				throw;
			}
		}

		internal async Task<HttpRequestContent> ExtractHttpRequestContents(HttpRequestMessage request)
		{
			return new HttpRequestContent(
				method: request?.Method.Method,
				uri: request?.RequestUri.AbsoluteUri,
				body: await ProcessRequestBody(request?.Content).ConfigureAwait(false),
				headers: request?.Headers == null
					? request?.Content?.Headers?.ToImmutableDictionary(x => x.Key, elementSelector: FlattenHeaderValue)
					: request?.Headers?.ToImmutableDictionary(x => x.Key, elementSelector: FlattenHeaderValue),
				timeStart: DateTime.Now
			);
		}

		private string FlattenHeaderValue(KeyValuePair<string, IEnumerable<string>> header)
		{
			return string.Join(" ", header.Value);
		}

		internal async Task<HttpResponseContent> ExtractHttpResponseContents(HttpResponseMessage response)
		{
			return new HttpResponseContent(
				statusCode: response.StatusCode,
				body: await ProcessResponseBody(response?.Content).ConfigureAwait(false),
				headers: response?.Headers?.ToImmutableDictionary(x => x.Key, elementSelector: FlattenHeaderValue),
				timeReceived: DateTime.Now
			);
		}

		protected virtual Task<string> ProcessRequestBody(HttpContent requestContent)
			=> requestContent?.ReadAsStringAsync() ?? Task.FromResult(string.Empty);

		protected virtual Task<string> ProcessResponseBody(HttpContent responseContent)
			=> responseContent?.ReadAsStringAsync() ?? Task.FromResult(string.Empty);
	}
}
