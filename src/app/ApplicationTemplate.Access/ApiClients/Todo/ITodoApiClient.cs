using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace ApplicationTemplate.DataAccess;

/// <summary>
/// Provides access to the Todo API.
/// </summary>
public interface ITodoApiClient
{
	/// <summary>
	/// Retrieves a list of all todo items.
	/// </summary>
	/// <param name="ct">The cancellation token.</param>
	/// <returns>A list of todo items.</returns>
	[Get("/todos")]
	Task<List<TodoData>> GetAllTodos(CancellationToken ct);

	/// <summary>
	/// Retrieves a single todo item by its ID.
	/// </summary>
	/// <param name="ct">The cancellation token.</param>
	/// <param name="id">The ID of the todo item.</param>
	/// <returns>The todo item.</returns>
	[Get("/todos/{id}")]
	Task<TodoData> GetTodoById(CancellationToken ct, int id);
}
