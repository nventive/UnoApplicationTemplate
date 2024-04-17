using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationTemplate.DataAccess;

public partial class DadJokesErrorResponse
{
	public DadJokesErrorResponse(string message)
	{
		Message = message;
	}

	public string Message { get; }
}
