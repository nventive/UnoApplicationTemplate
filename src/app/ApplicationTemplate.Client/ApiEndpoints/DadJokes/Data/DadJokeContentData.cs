using System;
using System.Collections;
using System.Text;
using Uno;

namespace ApplicationTemplate.Client
{
	[GeneratedImmutable]
	public partial class DadJokeContentData
	{
		public DadJokeContentData(string id, string title, string selftext, int total_awards_received, string distinguished, bool isFavorite)
		{
			Id = id;
			Title = title;
			Selftext = selftext;
			TotalAwardsReceived = total_awards_received;
			Distinguished = distinguished;
			IsFavorite = isFavorite;
		}

		[EqualityKey]
		public string Id { get; }

		public string Title { get; }

		public string Selftext { get; }

		public int TotalAwardsReceived { get; }

		public string Distinguished { get; }

		public bool IsFavorite { get; }
	}
}
