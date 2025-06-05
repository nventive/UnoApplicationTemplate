using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationTemplate.DataAccess;

/// <summary>
/// Mock implementation of <see cref="ITodoApiClient"/> for testing and development.
/// </summary>
public class TodoApiClientMock : ITodoApiClient
{
	public Task<List<TodoData>> GetTodos(CancellationToken ct)
	{
		// Return a mock list based on sample JSON.
		var mockTodos = new List<TodoData>
		{
			new TodoData(1, 1, "delectus aut autem", false),
			new TodoData(1, 2, "quis ut nam facilis et officia qui", false)
		};
		return Task.FromResult(mockTodos);
	}

	public Task<TodoData> GetTodo(CancellationToken ct, int id)
	{
		// Return a mock item; in a real mock, you could add logic for different IDs.
		return Task.FromResult(new TodoData(1, id, "delectus aut autem", false));
	}
}
