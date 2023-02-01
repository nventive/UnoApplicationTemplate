using System.Text.Json.Serialization;

namespace ApplicationTemplate.Client;

public sealed class UserProfileData
{
	public UserProfileData(string id, string firstName, string lastName, string email)
	{
		Id = id;
		FirstName = firstName;
		LastName = lastName;
		Email = email;
	}

	[JsonConverter(typeof(StringJsonConverter))]
	public string Id { get; }

	public string FirstName { get; }

	public string LastName { get; }

	public string Email { get; }
}
