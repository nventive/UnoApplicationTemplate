using System;
using System.Collections.Generic;
using System.Text;
using ApplicationTemplate.Client;
using Uno;

namespace ApplicationTemplate.Business
{
	[GeneratedImmutable]
	public partial class DadJokesQuote
	{
		public DadJokesQuote(DadJokeContentData data, bool isFavorite)
		{
			if (data.Distinguished == "moderator")
			{
				return;
			}

			if (data is null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			Id = data.Id;
			Selftext = data.Selftext;
			Title = data.Title;
			TotalAwardsReceived = data.TotalAwardsReceived;
			Distinguished = data.Distinguished;
			IsFavorite = isFavorite;
		}

		[EqualityKey]
		[EqualityHash]
		public string Id { get; }

		public string Selftext { get; }

		public string Title { get; }

		public bool HasAwards { get; }

		public string Distinguished { get; }

		public int TotalAwardsReceived { get; }

		[EqualityIgnore]
		public bool IsFavorite { get; }
	}
}
