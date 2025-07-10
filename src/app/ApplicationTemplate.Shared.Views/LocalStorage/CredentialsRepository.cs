// src/app/ApplicationTemplate.Shared.Views/DataAccess/LocalStorage/CredentialsRepository.cs
using System.Threading.Tasks;
#if __WINDOWS__
using Windows.Security.Credentials;
#endif
#if __IOS__
using Security;
using Foundation;
#endif
#if __ANDROID__
using Android.Content;
using Android.Preferences;
#endif

namespace ApplicationTemplate.DataAccess.LocalStorage;

public class CredentialsRepository : ICredentialsRepository
{
	public Task<Credentials> Read(string resource, string username)
	{
#if __WINDOWS__
		var vault = new PasswordVault();
		try
		{
			var cred = vault.Retrieve(resource, username);
			return Task.FromResult(new Credentials { Username = cred.UserName, Password = cred.Password });
		}
		catch
		{
			return Task.FromResult<Credentials>(null);
		}
#elif __IOS__
		var query = new SecRecord(SecKind.GenericPassword)
		{
			Service = resource,
			Account = username
		};
		SecStatusCode status;
		var data = SecKeyChain.QueryAsData(query, false, out status);
		if (status == SecStatusCode.Success)
		{
			var password = NSString.FromData(data, NSStringEncoding.UTF8).ToString();
			return Task.FromResult(new Credentials { Username = username, Password = password });
		}
		return Task.FromResult<Credentials>(null);
#elif __ANDROID__
		var prefs = PreferenceManager.GetDefaultSharedPreferences(Android.App.Application.Context);
		var key = $"{resource}_{username}";
		var password = prefs.GetString(key, null);
		if (password != null)
		{
			return Task.FromResult(new Credentials { Username = username, Password = password });
		}
		return Task.FromResult<Credentials>(null);
#else
		return Task.FromResult<Credentials>(null);
#endif
	}

	public Task Write(string resource, Credentials credentials)
	{
#if __WINDOWS__
		var vault = new PasswordVault();
		vault.Add(new PasswordCredential(resource, credentials.Username, credentials.Password));
		return Task.CompletedTask;
#elif __IOS__
		var record = new SecRecord(SecKind.GenericPassword)
		{
			Service = resource,
			Account = credentials.Username,
			ValueData = NSData.FromString(credentials.Password, NSStringEncoding.UTF8)
		};
		var status = SecKeyChain.Add(record);
		return Task.CompletedTask;
#elif __ANDROID__
		var prefs = PreferenceManager.GetDefaultSharedPreferences(Android.App.Application.Context);
		var key = $"{resource}_{credentials.Username}";
		var editor = prefs.Edit();
		editor.PutString(key, credentials.Password);
		editor.Commit();
		return Task.CompletedTask;
#else
		return Task.CompletedTask;
#endif
	}
}
