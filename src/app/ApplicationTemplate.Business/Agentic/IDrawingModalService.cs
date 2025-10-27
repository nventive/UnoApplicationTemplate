using System.Threading;
using System.Threading.Tasks;

namespace ApplicationTemplate.Business.Agentic;

/// <summary>
/// Service for displaying drawing content in a modal.
/// Allows the AI agent to visualize drawings, diagrams, or illustrations.
/// </summary>
public interface IDrawingModalService
{
	/// <summary>
	/// Shows a modal with SVG drawing content.
	/// </summary>
	/// <param name="svgContent">The SVG content to display (can be path commands or full SVG markup).</param>
	/// <param name="title">The title of the drawing modal.</param>
	/// <param name="description">Optional description or caption for the drawing.</param>
	/// <param name="ct">Cancellation token.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	Task ShowDrawingAsync(string svgContent, string title, string description = null, CancellationToken ct = default);
}
