using System.Text.Json.Serialization;  // For potential JSON attribute usage if needed

namespace ApplicationTemplate.DataAccess;

/// <summary>
/// Represents a Todo item from the API.
/// </summary>
public sealed class TodoData
{
	/// <summary>
	/// Initializes a new instance of the <see cref="TodoData"/> class.
	/// </summary>
	/// <param name="userId">The ID of the user.</param>
	/// <param name="id">The ID of the todo item.</param>
	/// <param name="title">The title of the todo item.</param>
	/// <param name="completed">Whether the todo item is completed.</param>
	public TodoData(int userId, int id, string title, bool completed)
	{
		UserId = userId;
		Id = id;
		Title = title;
		Completed = completed;
	}

	/// <summary>
	/// Gets the user ID.
	/// </summary>
	[JsonPropertyName("userId")]  // Matches JSON key exactly; adjust if using different serializer
	public int UserId { get; }

	/// <summary>
	/// Gets the todo item ID.
	/// </summary>
	[JsonPropertyName("id")]
	public int Id { get; }

	/// <summary>
	/// Gets the title of the todo item.
	/// </summary>
	[JsonPropertyName("title")]
	public string Title { get; }

	/// <summary>
	/// Gets a value indicating whether the todo item is completed.
	/// </summary>
	[JsonPropertyName("completed")]
	public bool Completed { get; }
}
