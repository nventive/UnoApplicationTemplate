using System;

namespace ApplicationTemplate
{
	public class OnboardingItemViewModel
	{
		public OnboardingItemViewModel(string primaryText, string imageUrl)
		{
			PrimaryText = primaryText;
			ImageUrl = imageUrl;
		}

		public string PrimaryText { get; }

		public string ImageUrl { get; }
	}
}
