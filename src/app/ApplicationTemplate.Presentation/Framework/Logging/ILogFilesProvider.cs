using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationTemplate
{
	public interface ILogFilesProvider
	{
		string[] GetLogFilesPaths();

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
}
