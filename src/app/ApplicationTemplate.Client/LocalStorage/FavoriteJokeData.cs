using System;
using System.Collections.Generic;
using System.Text;
using Uno;

namespace ApplicationTemplate.Client
{
	[GeneratedImmutable]
	public partial class FavoriteJokeData
	{
		public FavoriteJokeData(string id, string title, string selftext, int totalAwardsReceived, string distinguished)
		{
			Id = id;
			Title = title;
			Selftext = selftext;
			TotalAwardsReceived = totalAwardsReceived;
			Distinguished = distinguished;
		}

		[EqualityKey]
		public string Id { get; }

		public string Title { get; }

		public string Selftext { get; }

		public int TotalAwardsReceived { get; }

		public string Distinguished { get; }
	}
}
