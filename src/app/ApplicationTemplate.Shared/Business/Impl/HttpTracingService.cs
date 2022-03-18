using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Subjects;
using ApplicationTemplate.Client;

namespace ApplicationTemplate.Business
{
	public class HttpTracingService : IHttpTracingService
	{
		public ImmutableSortedDictionary<Guid, HttpLog> Logs { get; private set; } = ImmutableSortedDictionary<Guid, HttpLog>.Empty;

		private ISubject<IEnumerable<HttpLog>> _subject = new Subject<IEnumerable<HttpLog>>();

		public IObservable<IEnumerable<HttpLog>> ObserveLogs() => _subject;

		public Guid AddLog(HttpLog log)
		{
			lock (Logs)
			{
				Guid id = Guid.NewGuid();
				Logs = Logs.Add(id, log);
				_subject.OnNext(Logs.Values);
				return id;
			}
		}

		public void UpdateLog(Guid id, HttpLog log)
		{
			lock (Logs)
			{
				Logs = Logs.SetItem(id, log);
				_subject.OnNext(Logs.Values);
			}
		}
	}
}
