using System;
using System.Collections.Generic;
using System.Text;
using Uno;

namespace ApplicationTemplate.Client
{
	[GeneratedImmutable]
	public partial class UserProfileData
	{
		[EqualityKey]
		public string Id { get; }

		public string FirstName { get; }

		public string LastName { get; }

		public string Email { get; }
	}
}
