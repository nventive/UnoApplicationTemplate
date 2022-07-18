using System;
using System.Collections.Immutable;

namespace ApplicationTemplate.Client
{
	public record ApplicationSettings
	{
		public static ApplicationSettings Default { get; } = new ApplicationSettings();

		public ApplicationSettings()
		{
		}

		public ApplicationSettings(AuthenticationData authenticationData, bool isOnboardingCompleted, ImmutableDictionary<string, FavoriteJokeData> favoriteQuotes)
		{
			AuthenticationData = authenticationData;
			IsOnboardingCompleted = isOnboardingCompleted;
			FavoriteQuotes = favoriteQuotes;
		}

		public AuthenticationData AuthenticationData { get; init; }

		public bool IsOnboardingCompleted { get; init; }

		public ImmutableDictionary<string, FavoriteJokeData> FavoriteQuotes { get; init; } = ImmutableDictionary<string, FavoriteJokeData>.Empty;
	}
}
