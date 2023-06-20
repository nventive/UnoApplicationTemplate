using Chinook.DynamicMvvm;
using ReviewService;
using ReviewService.Abstractions;
using Uno;

namespace ApplicationTemplate.Presentation;
public sealed partial class StarRatingTestingPageViewModel : ViewModel
{
	[Inject] private IReviewService<ReviewSettingsCustom> _reviewService;

	public IDynamicCommand TestReviewRequest => this.GetCommandFromTask(async ct =>
	{
		await _reviewService.TryRequestReview(ct);
	});

	public IDynamicCommand TestApplicationLaunched => this.GetCommandFromTask(async ct =>
	{
		await _reviewService.TrackApplicationLaunched(ct);
	});

	public IDynamicCommand TestPrimaryActionCount => this.GetCommandFromTask(async ct =>
	{
		await _reviewService.TrackPrimaryActionCompleted(ct);
	});

	public IDynamicCommand TestSecondaryActionCount => this.GetCommandFromTask(async ct =>
	{
		await _reviewService.TrackSecondaryActionCompleted(ct);
	});
}
