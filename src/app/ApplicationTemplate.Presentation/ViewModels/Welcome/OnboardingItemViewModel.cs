using System;
using System.ComponentModel;

namespace ApplicationTemplate.Presentation;

[Bindable(true)]
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
