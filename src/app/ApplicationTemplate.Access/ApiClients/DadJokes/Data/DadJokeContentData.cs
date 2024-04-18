using System;
using System.Collections;
using System.Text;
using Uno;

namespace ApplicationTemplate.DataAccess;

public class DadJokeContentData
{
	public DadJokeContentData(string id, string title, string selftext, int totalAwardsReceived, string distinguished, bool isFavorite)
	{
		Id = id;
		Title = title;
		Selftext = selftext;
		TotalAwardsReceived = totalAwardsReceived;
		Distinguished = distinguished;
		IsFavorite = isFavorite;
	}

	public string Id { get; }

	public string Title { get; }

	public string Selftext { get; }

	public int TotalAwardsReceived { get; }

	public string Distinguished { get; }

	public bool IsFavorite { get; }
}
