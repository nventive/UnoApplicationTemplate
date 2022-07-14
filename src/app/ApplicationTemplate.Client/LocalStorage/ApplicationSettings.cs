using System;
using System.Collections.Immutable;
using Uno;

namespace ApplicationTemplate.Client
{
	[GeneratedImmutable]
	public partial class ApplicationSettings
	{
		[EqualityHash]
		public AuthenticationData AuthenticationData { get; }

		public bool IsOnboardingCompleted { get; }

		public ImmutableDictionary<string, FavoriteJokeData> FavoriteQuotes { get; } = ImmutableDictionary<string, FavoriteJokeData>.Empty;
	}
}
