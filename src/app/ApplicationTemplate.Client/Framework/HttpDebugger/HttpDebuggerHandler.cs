using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationTemplate.Client;

public class HttpDebuggerHandler : DelegatingHandler
{
	private IHttpDebuggerService _debuggerService;

	public HttpDebuggerHandler(IHttpDebuggerService debuggerService)
	{
		_debuggerService = debuggerService;
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var trace = await _debuggerService.StartTrace(request);
		var timer = Stopwatch.StartNew();
		try
		{
			var response = await base.SendAsync(request, cancellationToken);
			timer.Stop();
			_debuggerService.UpdateTrace(trace with
			{
				ElapsedTime = timer.Elapsed,
				Status = HttpTraceStatus.Received,
				StatusCode = response.StatusCode,
				ReasonPhrase = response.ReasonPhrase,
				ResponseVersion = response.Version,
				ResponseHeaders = response.Headers,
				ResponseContentHeaders = response.Content.Headers,
				ResponseContent = await response.Content.ReadAsStringAsync(),
			});

			return response;
		}
		catch (Exception exception)
		{
			timer.Stop();
			_debuggerService.UpdateTrace(trace with
			{
				ElapsedTime = timer.Elapsed,
				Exception = exception,
				Status = exception is OperationCanceledException ? HttpTraceStatus.Cancelled : HttpTraceStatus.Failed
			});
			throw;
		}
	}
}
