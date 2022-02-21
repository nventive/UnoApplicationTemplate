using System;
using System.Collections.Generic;
using System.Text;
using GeneratedSerializers;
using Uno;

namespace ApplicationTemplate.Client
{
	[GeneratedImmutable]
	public partial class DadJokesData
	{

		public DadJokesData(DadJokeChildData[] children)
		{
			Children = children;
		}

		[EqualityHash]
		[SerializationProperty("children")]
		public DadJokeChildData[] Children { get; }
	}
}
