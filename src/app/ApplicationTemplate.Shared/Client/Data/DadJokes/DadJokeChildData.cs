using System;
using System.Collections.Generic;
using System.Text;
using Uno;

namespace ApplicationTemplate.Client;

[GeneratedImmutable]
public partial class DadJokeChildData
{
	public DadJokeChildData(DadJokeContentData data)
	{
		Data = data;
	}

	[EqualityHash]
	public DadJokeContentData Data { get; }
}
