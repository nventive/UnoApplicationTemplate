using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;

namespace ApplicationTemplate.Business;

/// <summary>
/// Provides access to the dad jokes.
/// </summary>
public interface IDadJokesService
{
	/// <summary>
	/// Gets and observes the post type filter.
	/// </summary>
	ReplaySubject<PostTypes> GetAndObservePostTypeFilter();

	/// <summary>
	/// Sets the post type filter.
	/// </summary>
	/// <param name="pt">The <see cref="PostTypes"/>.</param>
	void SetPostTypeFilter(PostTypes pt);

	/// <summary>
	/// Returns a list of dad jokes.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <returns>A list of quotes.</returns>
	Task<DadJokesQuote[]> FetchData(CancellationToken ct);

	/// <summary>
	/// Gets the list of favorite quotes.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <returns>An observable list of favorite quotes.</returns>
	Task<IObservableList<DadJokesQuote>> GetFavorites(CancellationToken ct);

	/// <summary>
	/// Sets whether or not a quote is favorite.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <param name="quote">The <see cref="DadJokesQuote"/> to set.</param>
	/// <param name="isFavorite">Whether the quote is a favorite or not.</param>
	Task SetIsFavorite(CancellationToken ct, DadJokesQuote quote, bool isFavorite);
}
