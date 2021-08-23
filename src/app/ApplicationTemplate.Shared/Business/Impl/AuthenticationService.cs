using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Client;

namespace ApplicationTemplate.Business
{
	public partial class AuthenticationService : IAuthenticationService
	{
		private readonly IApplicationSettingsService _applicationSettingsService;
		private readonly IAuthenticationEndpoint _authenticationEndpoint;
		private readonly INotifySessionExpired _notifySessionExpired;

		public AuthenticationService(
			IApplicationSettingsService applicationSettingsService,
			IAuthenticationEndpoint authenticationEndpoint,
			INotifySessionExpired notifySessionExpired)
		{
			_applicationSettingsService = applicationSettingsService ?? throw new ArgumentNullException(nameof(applicationSettingsService));
			_authenticationEndpoint = authenticationEndpoint ?? throw new ArgumentNullException(nameof(authenticationEndpoint));
			_notifySessionExpired = notifySessionExpired ?? throw new ArgumentNullException(nameof(notifySessionExpired));
		}

		/// <inheritdoc/>
		public IObservable<AuthenticationData> GetAndObserveAuthenticationData()
		{
			return _applicationSettingsService
				.GetAndObserveCurrent()
				.Select(s => s.AuthenticationData);
		}

		/// <inheritdoc/>
		public IObservable<bool> GetAndObserveIsAuthenticated()
		{
			return GetAndObserveAuthenticationData()
				.Select(s => s != default(AuthenticationData));
		}

		/// <inheritdoc/>
		public IObservable<Unit> ObserveSessionExpired() => _notifySessionExpired.ObserveSessionExpired();

		/// <inheritdoc/>
		public async Task<AuthenticationData> Login(CancellationToken ct, string email, string password)
		{
			var authenticationData = await _authenticationEndpoint.Login(ct, email, password);

			await _applicationSettingsService.SetAuthenticationData(ct, authenticationData);

			return authenticationData;
		}

		/// <inheritdoc/>
		public async Task Logout(CancellationToken ct)
		{
			await _applicationSettingsService.DiscardUserSettings(ct);
		}

		/// <inheritdoc/>
		public async Task CreateAccount(CancellationToken ct, string email, string password)
		{
			await _authenticationEndpoint.CreateAccount(ct, email, password);
		}

		/// <inheritdoc/>
		public async Task ResetPassword(CancellationToken ct, string email)
		{
			await _authenticationEndpoint.ResetPassword(ct, email);
		}
	}
}
