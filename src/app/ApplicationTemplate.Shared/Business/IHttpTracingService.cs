using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using ApplicationTemplate.Client;

namespace ApplicationTemplate.Business
{
	public interface IHttpTracingService
	{
		ImmutableSortedDictionary<Guid, HttpLog> Logs { get; }

		Guid AddLog(HttpLog log);

		void UpdateLog(Guid id, HttpLog log);

		IObservable<IEnumerable<HttpLog>> ObserveLogs();
	}
}
