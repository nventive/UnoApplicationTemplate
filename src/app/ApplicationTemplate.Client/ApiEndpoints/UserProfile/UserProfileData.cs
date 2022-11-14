using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ApplicationTemplate.Client;

//public record UserProfileData
//{
//	public UserProfileData(string id, string firstName, string lastName, string email)
//	{
//		Id = id;
//		FirstName = firstName;
//		LastName = lastName;
//		Email = email;
//	}

//	[JsonConverter(typeof(StringJsonConverter))]
//	public string Id { get; }

//	public string FirstName { get; init; }

//	public string LastName { get; init; }

//	public string Email { get; init; }
//}

public class UserProfileData
{
	public UserProfileData()
	{
	}

	public UserProfileData(string id, string firstName, string lastName, string email)
	{
		Id = id;
		FirstName = firstName;
		LastName = lastName;
		Email = email;
	}

	[JsonConverter(typeof(StringJsonConverter))]
	public string Id { get; set; }

	public string FirstName { get; set; }

	public string LastName { get; set; }

	public string Email { get; set; }
}
