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
		var result = await apiClient.GetAllTodos(ct);

		// Assert
		Assert.NotEmpty(result);
		Assert.All(result, todo => Assert.True(todo.Id > 0));  // Basic validation based on expected data
	}

	[Theory]
	[InlineData(1)]  // Test with a known ID from the sample JSON
	public async Task GetTodoById_ReturnsTodo(int id)
	{
		// Arrange
		var apiClient = GetService<ITodoApiClient>();
		var ct = CancellationToken.None;

		// Act
		var result = await apiClient.GetTodoById(ct, id);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(id, result.Id);
		Assert.Equal(1, result.UserId);  // Based on sample JSON
		Assert.False(result.Completed);  // Based on sample JSON
	}
}
