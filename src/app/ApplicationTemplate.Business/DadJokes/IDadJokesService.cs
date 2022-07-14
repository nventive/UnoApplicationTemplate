using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Client;
using DynamicData;

namespace ApplicationTemplate.Business
{
	public interface IDadJokesService
	{
		/// <summary>
		/// Returns a list of dad jokes.
		/// </summary>
		/// <returns>List of quotes</returns>
		ReplaySubject<PostTypes> GetAndObservePostTypeFilter();

		/// <summary>
		/// Set post type.
		/// </summary>
		/// <param name="pt"><see cref="PostTypes"/></param>
		void SetPostTypeFilter(PostTypes pt);

		/// <summary>
		/// Returns a list of dad jokes.
		/// </summary>
		/// <param name="ct"><see cref="CancellationToken"/></param>
		/// <returns>List of quotes</returns>
		Task<DadJokesQuote[]> FetchData(CancellationToken ct);

		/// <summary>
		/// Returns the list of favorite quotes.
		/// </summary>
		/// <param name="ct"><see cref="CancellationToken"/></param>
		/// <returns>List of favorite quotes</returns>
		Task<IObservableList<DadJokesQuote>> GetFavorites(CancellationToken ct);

		/// <summary>
		/// Sets whether or not a quote is favorite.
		/// </summary>
		/// <param name="ct"><see cref="CancellationToken"/></param>
		/// <param name="quote"><see cref="DadJokesQuote"/></param>
		/// <param name="isFavorite">Is favorite or not</param>
		/// <returns><see cref="Task"/></returns>
		Task SetIsFavorite(CancellationToken ct, DadJokesQuote quote, bool isFavorite);
	}
}
