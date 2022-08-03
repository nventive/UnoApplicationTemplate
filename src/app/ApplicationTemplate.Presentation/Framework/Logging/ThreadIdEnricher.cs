using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using Serilog.Core;
using Serilog.Events;

namespace ApplicationTemplate
{
	/// <summary>
	/// This <see cref="ILogEventEnricher"/> adds a "ThreadId" property containing the ManagedThreadId of the current thread.
	/// </summary>
	public class ThreadIdEnricher : ILogEventEnricher
	{
		public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
		{
			logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ThreadId", Environment.CurrentManagedThreadId.ToString("D2", CultureInfo.InvariantCulture)));
		}
	}
}
