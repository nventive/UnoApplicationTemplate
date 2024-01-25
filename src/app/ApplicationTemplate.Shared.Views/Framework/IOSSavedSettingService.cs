#if IOS
using System;
using System.Collections.Generic;
using System.Threading;
using Foundation;

namespace ApplicationTemplate.Views
{
	public class IOSSavedSettingService : ISavedSettingsService
	{
		public void LoginSaved()
		{
		}

		public void DeleteSaved(string key)
		{
			var store = NSUbiquitousKeyValueStore.DefaultStore;
			store.Remove(key);
			store.Synchronize();
		}

		public string ReadSavedString(string key)
		{
			var store = NSUbiquitousKeyValueStore.DefaultStore;
			return store.GetString(key);
		}

		public void SetSavedString(string key, string data)
		{
			var store = NSUbiquitousKeyValueStore.DefaultStore;
			store.SetString(data, key);
			store.Synchronize();
		}
	}
}
#endif
