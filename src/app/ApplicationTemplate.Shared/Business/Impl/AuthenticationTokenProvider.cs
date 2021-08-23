using System;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Client;
using MallardMessageHandlers;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Business
{
	public class AuthenticationTokenProvider : IAuthenticationTokenProvider<AuthenticationData>, INotifySessionExpired
	{
		private readonly ISubject<Unit> _sessionExpired = new Subject<Unit>();

		private readonly IApplicationSettingsService _applicationSettingsService;
		private readonly IAuthenticationEndpoint _authenticationEndpoint;
		private readonly IAuthenticationTokenProvider<AuthenticationData> _inner;

		public AuthenticationTokenProvider(IApplicationSettingsService applicationSettingsService, IAuthenticationEndpoint authenticationEndpoint, ILoggerFactory loggerFactory)
		{
			_applicationSettingsService = applicationSettingsService;
			_authenticationEndpoint = authenticationEndpoint;
			_inner = new ConcurrentAuthenticationTokenProvider<AuthenticationData>(loggerFactory, GetToken, NotifySessionExpired, RefreshToken);
		}

		/// <inheritdoc />
		public IObservable<Unit> ObserveSessionExpired() => _sessionExpired;

		/// <inheritdoc />
		Task<AuthenticationData> IAuthenticationTokenProvider<AuthenticationData>.GetToken(CancellationToken ct, HttpRequestMessage request)
			=> _inner.GetToken(ct, request);

		/// <inheritdoc />
		Task<AuthenticationData> IAuthenticationTokenProvider<AuthenticationData>.RefreshToken(CancellationToken ct, HttpRequestMessage request, AuthenticationData unauthorizedToken)
			=> _inner.RefreshToken(ct, request, unauthorizedToken);

		/// <inheritdoc/>
		Task IAuthenticationTokenProvider<AuthenticationData>.NotifySessionExpired(CancellationToken ct, HttpRequestMessage request, AuthenticationData unauthorizedToken)
			=> _inner.NotifySessionExpired(ct, request, unauthorizedToken);

		private async Task<AuthenticationData> GetToken(CancellationToken ct, HttpRequestMessage request)
		{
			var settings = await _applicationSettingsService.GetAndObserveCurrent().FirstAsync(ct);

			return settings.AuthenticationData;
		}

		private async Task NotifySessionExpired(CancellationToken ct, HttpRequestMessage request, AuthenticationData unauthorizedToken)
		{
			await _applicationSettingsService.SetAuthenticationData(ct, default(AuthenticationData));

			_sessionExpired.OnNext(Unit.Default);
		}

		private async Task<AuthenticationData> RefreshToken(CancellationToken ct, HttpRequestMessage request, AuthenticationData unauthorizedToken)
		{
			var authenticationData = await _authenticationEndpoint.RefreshToken(ct, unauthorizedToken);

			await _applicationSettingsService.SetAuthenticationData(ct, authenticationData);

			return authenticationData;
		}
	}
}
