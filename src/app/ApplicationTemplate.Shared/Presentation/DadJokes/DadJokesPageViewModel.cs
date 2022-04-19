using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chinook.DataLoader;
using Chinook.DynamicMvvm;
using Chinook.StackNavigation;
using DynamicData;

namespace ApplicationTemplate.Presentation
{
	public partial class DadJokesPageViewModel : ViewModel
	{
		public IDynamicCommand RefreshJokes => this.GetCommandFromDataLoaderRefresh(Jokes);

		public IDataLoader<int[]> Jokes => this.GetDataLoader(LoadJokes, b => b
			// Dispose the previous ItemViewModels when Quotes produces new values
			.DisposePreviousData()
			.TriggerOnNetworkReconnection()
		);

		private async Task<int[]> LoadJokes(CancellationToken ct, IDataLoaderRequest request)
		{
			return new int[] { 1,2,3,4,5 };
		}
	}
}
