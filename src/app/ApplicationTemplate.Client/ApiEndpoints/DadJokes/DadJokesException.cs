using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationTemplate.Client;

public class DadJokesException : Exception
{
	public DadJokesException()
	{
	}

	public DadJokesException(string message)
		: base(message)
	{
	}

	public DadJokesException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
