#if ANDROID
using System;
using AndroidX.Credentials;
using AndroidX.Credentials.Exceptions;
using Java.Interop;
using Xamarin.GoogleAndroid.Libraries.Identity.GoogleId;

namespace ApplicationTemplate.Views
{
	public sealed class CredentialManagerCallback : Java.Lang.Object, ICredentialManagerCallback
	{
		public void OnError(Java.Lang.Object e)
		{
			Console.WriteLine(e);
		}

		public void OnResult(Java.Lang.Object result)
		{
			if (result is GetCredentialResponse)
			{
				HandleSignIn((GetCredentialResponse)result);
			}
		}

		private void HandleSignIn(GetCredentialResponse result)
		{
			// Handle the successfully returned credential.
			Credential credential = result.Credential;

			if (credential is PublicKeyCredential)
			{
				string responseJson = ((PublicKeyCredential)credential).AuthenticationResponseJson;
				// Share responseJson i.e. a GetCredentialResponse on your server to validate and authenticate
			}
			else if (credential is PasswordCredential)
			{
				string username = ((PasswordCredential)credential).Id;
				string password = ((PasswordCredential)credential).Password;
				// Use id and password to send to your server to validate and authenticate
			}
			else if (credential is CustomCredential)
			{
				if (credential.Type == GoogleIdTokenCredential.TypeGoogleIdTokenCredential)
				{
					try
					{
						// Use googleIdTokenCredential and extract id to validate and
						// authenticate on your server
						GoogleIdTokenCredential googleIdTokenCredential = GoogleIdTokenCredential.CreateFrom(((CustomCredential)credential).Data);
					}
					catch (GoogleIdTokenParsingException e)
					{
						Console.WriteLine("Received an invalid Google ID token response: " + e);
					}
				}
				else
				{
					// Catch any unrecognized custom credential type here.
					Console.WriteLine("Unexpected type of credential");
				}
			}
			else
			{
				// Catch any unrecognized credential type here.
				Console.WriteLine("Unexpected type of credential");
			}
		}
	}
}
#endif
