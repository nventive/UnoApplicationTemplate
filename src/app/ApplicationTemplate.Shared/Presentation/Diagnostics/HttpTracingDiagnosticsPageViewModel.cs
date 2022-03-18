using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using ApplicationTemplate.Business;
using Chinook.DynamicMvvm;
using Uno;

namespace ApplicationTemplate.Presentation
{
	public partial class HttpTracingDiagnosticsPageViewModel : ViewModel
	{
		[Inject] private IHttpTracingService _httpTracingService;

		public ObservableCollection<HttpLogItemViewModel> Logs => this.GetObservableCollectionFromObservable(
			source: _httpTracingService
				.ObserveLogs()
				.Select(logs =>
					logs.Select(log => this.GetChild(() => new HttpLogItemViewModel(log), log.Id.ToString()))
						.ToList()
				),
			initialValue: _httpTracingService.Logs.Values
				.Select(log => this.GetChild(() => new HttpLogItemViewModel(log), log.Id.ToString()))
				.ToList()
		);
	}
}
