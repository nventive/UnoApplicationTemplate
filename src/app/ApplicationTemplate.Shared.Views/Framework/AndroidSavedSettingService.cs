#if ANDROID

using System;
using AndroidX.Credentials;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Java.Util.Concurrent;
using Xamarin.GoogleAndroid.Libraries.Identity.GoogleId;

namespace ApplicationTemplate.Views
{
	public class AndroidSavedSettingService : ISavedSettingsService
	{
		public void LoginSaved()
		{
			GetGoogleIdOption googleIdOption = new GetGoogleIdOption.Builder()
				.SetFilterByAuthorizedAccounts(true)
				.SetAutoSelectEnabled(true)
				.SetServerClientId("1019407247826-c3cd4lgn1r7gjr2pctgk6v83q3usfs5q.apps.googleusercontent.com")
				.Build();

			GetCredentialRequest request = new GetCredentialRequest.Builder()
				.AddCredentialOption(googleIdOption)
				.Build();

			var credentialManager = CredentialManager.Create(Android.App.Application.Context);

			credentialManager.GetCredentialAsync(
				Android.App.Application.Context,
				request,
				null,
				Executors.NewSingleThreadExecutor(),
				new CredentialManagerCallback()
			);
		}

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
