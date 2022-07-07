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

namespace ApplicationTemplate.Business;

public partial class AuthenticationService : IAuthenticationService
{
	private readonly ISubject<Unit> _sessionExpired = new Subject<Unit>();

	private readonly IApplicationSettingsService _applicationSettingsService;
	private readonly IAuthenticationEndpoint _authenticationEndpoint;
	private readonly IAuthenticationTokenProvider<AuthenticationData> _authTokenProvider;

	public AuthenticationService(
		ILoggerFactory loggerFactory,
		IApplicationSettingsService applicationSettingsService,
		IAuthenticationEndpoint authenticationEndpoint)
	{
		_applicationSettingsService = applicationSettingsService ?? throw new ArgumentNullException(nameof(applicationSettingsService));
		_authenticationEndpoint = authenticationEndpoint ?? throw new ArgumentNullException(nameof(authenticationEndpoint));
		_authTokenProvider = new ConcurrentAuthenticationTokenProvider<AuthenticationData>(loggerFactory, GetTokenInternal, NotifySessionExpiredInternal, RefreshTokenInternal);
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
	public IObservable<Unit> ObserveSessionExpired() => _sessionExpired;

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

	/// <inheritdoc />
	Task<AuthenticationData> IAuthenticationTokenProvider<AuthenticationData>.GetToken(CancellationToken ct, HttpRequestMessage request)
		=> _authTokenProvider.GetToken(ct, request);

	/// <inheritdoc />
	Task<AuthenticationData> IAuthenticationTokenProvider<AuthenticationData>.RefreshToken(CancellationToken ct, HttpRequestMessage request, AuthenticationData unauthorizedToken)
		=> _authTokenProvider.RefreshToken(ct, request, unauthorizedToken);

	/// <inheritdoc/>
	Task IAuthenticationTokenProvider<AuthenticationData>.NotifySessionExpired(CancellationToken ct, HttpRequestMessage request, AuthenticationData unauthorizedToken)
		=> _authTokenProvider.NotifySessionExpired(ct, request, unauthorizedToken);

	private async Task<AuthenticationData> GetTokenInternal(CancellationToken ct, HttpRequestMessage request)
	{
		var settings = await _applicationSettingsService.GetAndObserveCurrent().FirstAsync(ct);

		return settings.AuthenticationData;
	}

	private async Task NotifySessionExpiredInternal(CancellationToken ct, HttpRequestMessage request, AuthenticationData unauthorizedToken)
	{
		await _applicationSettingsService.SetAuthenticationData(ct, default(AuthenticationData));

		_sessionExpired.OnNext(Unit.Default);
	}

	private async Task<AuthenticationData> RefreshTokenInternal(CancellationToken ct, HttpRequestMessage request, AuthenticationData unauthorizedToken)
	{
		var authenticationData = await _authenticationEndpoint.RefreshToken(ct, unauthorizedToken);

		await _applicationSettingsService.SetAuthenticationData(ct, authenticationData);

		return authenticationData;
	}
}
