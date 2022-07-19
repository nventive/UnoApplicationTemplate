using System;
using System.Collections.Immutable;

namespace ApplicationTemplate.Client
{
	public record ApplicationSettings
	{
		public static ApplicationSettings Default { get; } = new ApplicationSettings();

		public ApplicationSettings(
			AuthenticationData authenticationData = null,
			bool isOnboardingCompleted = false,
			ImmutableDictionary<string, FavoriteJokeData> favoriteQuotes = null)
		{
			AuthenticationData = authenticationData;
			IsOnboardingCompleted = isOnboardingCompleted;
			FavoriteQuotes = favoriteQuotes ?? ImmutableDictionary<string, FavoriteJokeData>.Empty;
		}

		public AuthenticationData AuthenticationData { get; init; }

		public bool IsOnboardingCompleted { get; init; }

		public ImmutableDictionary<string, FavoriteJokeData> FavoriteQuotes { get; init; } = ImmutableDictionary<string, FavoriteJokeData>.Empty;
	}
}
