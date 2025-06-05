using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.DataAccess;
using ApplicationTemplate.Tests;
using Xunit;

namespace ApplicationTemplate.Tests.Api;

/// <summary>
/// Tests for <see cref="ITodoApiClient"/>.
/// </summary>
public sealed class TodoApiClientShould : ApiTestBase
{
	[Fact]
	public async Task GetAllTodos_ReturnsTodos()
	{
		// Arrange
		var apiClient = GetService<ITodoApiClient>();
		var ct = CancellationToken.None;

		// Act
		var result = await apiClient.GetTodos(ct);

		// Assert
		Assert.NotNull(result);
		Assert.NotEmpty(result);
	}

	[Theory]
	[InlineData(1)]
	public async Task GetTodoById_ReturnsTodo(int id)
	{
		// Arrange
		var apiClient = GetService<ITodoApiClient>();
		var ct = CancellationToken.None;

		// Act
		var result = await apiClient.GetTodo(ct, id);

		// Assert
		// Validates the value because we know them from the mock data which comes from the API.
		Assert.NotNull(result);
		Assert.Equal(id, result.Id);
		Assert.Equal(1, result.UserId);
		Assert.Equal("delectus aut autem", result.Title);
		Assert.False(result.Completed);
	}
}
