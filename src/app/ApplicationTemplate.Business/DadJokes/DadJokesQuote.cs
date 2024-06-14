using System;
using System.Collections.Generic;
using System.Text;
using ApplicationTemplate.DataAccess;

namespace ApplicationTemplate.Business;

public record DadJokesQuote
{
	public DadJokesQuote(DadJokeContentData data, bool isFavorite)
	{
		if (data.Distinguished == "moderator")
		{
			return;
		}

		ArgumentNullException.ThrowIfNull(data);

		Id = data.Id;
		Selftext = data.Selftext;
		Title = data.Title;
		TotalAwardsReceived = data.TotalAwardsReceived;
		Distinguished = data.Distinguished;
		IsFavorite = isFavorite;
	}

	public DadJokesQuote(FavoriteJokeData favoriteJokeData)
	{
		Id = favoriteJokeData.Id;
		Selftext = favoriteJokeData.Selftext;
		Title = favoriteJokeData.Title;
		TotalAwardsReceived = favoriteJokeData.TotalAwardsReceived;
		Distinguished = favoriteJokeData.Distinguished;
		IsFavorite = true;
	}

	public string Id { get; }

	public string Selftext { get; }

	public string Title { get; }

	public bool HasAwards { get; }

	public string Distinguished { get; }

	public int TotalAwardsReceived { get; }

	public bool IsFavorite { get; init; }

	public FavoriteJokeData ToFavoriteJokeData()
	{
		return new FavoriteJokeData(Id, Title, Selftext, TotalAwardsReceived, Distinguished);
	}
}
