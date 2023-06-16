using System.Threading.Tasks;

namespace ReviewService.Abstractions;

/// <summary>
/// Provides ways to prompt user to review the current application.
/// </summary>
/// <remarks>
/// On iOS, you can't submit a review while developing, but the review prompt will be shown in your simulator or device.
///
/// On Android, you can't see the review prompt while developing or if you distribute the application manually.
/// You have to download the application from Google Play Store to see the review prompt.
///
/// See https://github.com/jamesmontemagno/StoreReviewPlugin for more details.
/// </remarks>
public interface IReviewPrompter
{
	/// <summary>
	/// Prompts the user to rate the current application using the platform's default application store.
	/// </summary>
	/// <returns><see cref="Task"/>.</returns>
	Task TryPrompt();
}
