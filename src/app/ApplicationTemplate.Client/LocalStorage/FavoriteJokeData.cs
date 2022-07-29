using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationTemplate.Client
{
	public class FavoriteJokeData
	{
		public FavoriteJokeData(string id, string title, string selftext, int totalAwardsReceived, string distinguished)
		{
			Id = id;
			Title = title;
			Selftext = selftext;
			TotalAwardsReceived = totalAwardsReceived;
			Distinguished = distinguished;
		}

		public string Id { get; }

		public string Title { get; }

		public string Selftext { get; }

		public int TotalAwardsReceived { get; }

		public string Distinguished { get; }
	}
}
