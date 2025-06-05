using System.Text.Json;
using ApplicationTemplate.DataAccess;
using Xunit;

namespace ApplicationTemplate.Tests.Unit;

public sealed class TodoDataTests
{
	[Fact]
	public void Deserialize_FromJson_MatchesExpectedValues()
	{
		// Arrange
		var json = "{\"userId\": 1, \"id\": 1, \"title\": \"delectus aut autem\", \"completed\": false}";  // Sample JSON from query

		// Act
		var todoData = JsonSerializer.Deserialize<TodoData>(json);

		// Assert
		Assert.NotNull(todoData);
		Assert.Equal(1, todoData.UserId);
		Assert.Equal(1, todoData.Id);
		Assert.Equal("delectus aut autem", todoData.Title);
		Assert.False(todoData.Completed);
	}
}
