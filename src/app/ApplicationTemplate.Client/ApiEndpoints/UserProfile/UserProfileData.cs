using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationTemplate.Client
{
	public record UserProfileData
	{
		public UserProfileData(string id, string firstName, string lastName, string email)
		{
			Id = id;
			FirstName = firstName;
			LastName = lastName;
			Email = email;
		}

		public string Id { get; }
				
		public string FirstName { get; init; }

		public string LastName { get; init; }

		public string Email { get; init; }
	}
}
