using Chinook.DynamicMvvm;
using ReviewService.Abstractions;
using Uno;

namespace ApplicationTemplate.Presentation.ViewModels;
public sealed partial class StarRatingTestingPageViewModel : ViewModel
{
	[Inject] private IReviewPrompter _reviewPrompter;

	public IDynamicCommand TestReviewRequest => this.GetCommand(() =>
	{
		_reviewPrompter.TryPrompt();
	});
}
