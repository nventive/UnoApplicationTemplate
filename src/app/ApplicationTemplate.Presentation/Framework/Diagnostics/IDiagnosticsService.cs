using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationTemplate
{
	public interface IDiagnosticsService
	{
		Task TestExceptionFromMainThread(CancellationToken ct);

		void OpenSettingsFolder();

		bool CanOpenSettingsFolder { get; }
	}
}
