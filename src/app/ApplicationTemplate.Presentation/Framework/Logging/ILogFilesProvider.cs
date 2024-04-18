using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationTemplate;

/// <summary>
/// Provides access to the log files.
/// This is useful to send the log files.
/// </summary>
public interface ILogFilesProvider
{
	/// <summary>
	/// Gets the paths of the log files.
	/// </summary>
	string[] GetLogFilesPaths();

	/// <summary>
	/// Deletes the log files.
	/// </summary>
	void DeleteLogFiles();
}

public class EmptyLogFilesProvider : ILogFilesProvider
{
	public string[] GetLogFilesPaths()
	{
		return Array.Empty<string>();
	}

	public void DeleteLogFiles()
	{
		return;
	}
}
