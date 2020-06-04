using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ApplicationTemplate
{
	public static class ConfigurationSettings
	{
		/// <summary>
		/// Gets whether or not a setting is enabled.
		/// This method checks the presence of a file on disk which
		/// is faster than resolving any service and deserialize the setting content.
		/// </summary>
		/// <param name="settingFilename">File name of the setting. This must be unique.</param>
		/// <returns>True if the setting is enabled, false otherwise.</returns>
		public static bool GetIsSettingEnabled(string settingFilename)
		{
			var filePath = GetSettingsFilePath(settingFilename);

			return File.Exists(filePath);
		}

		/// <summary>
		/// Sets whether or not a setting is enabled.
		/// This methods creates a file on disk if the setting is enabled
		/// and deletes it otherwise.
		/// </summary>
		/// <param name="settingFileName">File name of the setting. This must be unique.</param>
		/// <param name="isEnabled">Is the setting enabled</param>
		public static void SetIsSettingEnabled(string settingFileName, bool isEnabled)
		{
			var filePath = GetSettingsFilePath(settingFileName);

			if (isEnabled)
			{
				File.Create(filePath).Dispose();
			}
			else if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
		}

		private static string GetSettingsFilePath(string fileName)
		{
//-:cnd:noEmit
#if WINDOWS_UWP
//+:cnd:noEmit
			var folderPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
//-:cnd:noEmit
#elif __ANDROID__ || __IOS__
//+:cnd:noEmit
			var folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
//-:cnd:noEmit
#else
//+:cnd:noEmit
			var folderPath = string.Empty;
//-:cnd:noEmit
#endif
//+:cnd:noEmit

			return Path.Combine(folderPath, fileName);
		}
	}
}
