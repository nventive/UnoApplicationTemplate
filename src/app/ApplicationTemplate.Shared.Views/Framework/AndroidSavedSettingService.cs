#if ANDROID

using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace ApplicationTemplate.Views
{
	public class AndroidSavedSettingService : ISavedSettingsService
	{
		public void DeleteSaved(string key)
		{
		}

		public string ReadSavedString(string key)
		{
			return null;
		}

		public void SetSavedString(string key, string data)
		{
		}
	}
}

#endif
