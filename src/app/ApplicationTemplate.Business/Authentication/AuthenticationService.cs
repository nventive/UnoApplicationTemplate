using System;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.DataAccess;
using MallardMessageHandlers;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Business;

public partial class AuthenticationService : IAuthenticationService
{
	private readonly ISubject<Unit> _sessionExpired = new Subject<Unit>();

	private readonly IApplicationSettingsRepository _applicationSettingsRepository;
	private readonly IAuthenticationApiClient _authenticationApiClient;
	private readonly IAuthenticationTokenProvider<AuthenticationData> _authTokenProvider;

	public AuthenticationService(
		ILoggerFactory loggerFactory,
		IApplicationSettingsRepository applicationSettingsRepository,
		IAuthenticationApiClient authenticationApiClient)
	{
		_applicationSettingsRepository = applicationSettingsRepository ?? throw new ArgumentNullException(nameof(applicationSettingsRepository));
		_authenticationApiClient = authenticationApiClient ?? throw new ArgumentNullException(nameof(authenticationApiClient));
		_authTokenProvider = new ConcurrentAuthenticationTokenProvider<AuthenticationData>(loggerFactory, GetTokenInternal, NotifySessionExpiredInternal, RefreshTokenInternal);
	}

	/// <inheritdoc/>
	public IObservable<AuthenticationData> GetAndObserveAuthenticationData()
	{
		return _applicationSettingsRepository
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
		var authenticationData = await _authenticationApiClient.Login(ct, email, password);

		await _applicationSettingsRepository.SetAuthenticationData(ct, authenticationData);

		return authenticationData;
	}

	/// <inheritdoc/>
	public async Task Logout(CancellationToken ct)
	{
		await _applicationSettingsRepository.DiscardUserSettings(ct);
	}

	/// <inheritdoc/>
	public async Task<AuthenticationData> CreateAccount(CancellationToken ct, string email, string password)
	{
		var authenticationData = await _authenticationApiClient.CreateAccount(ct, email, password);

		await _applicationSettingsRepository.SetAuthenticationData(ct, authenticationData);

		return authenticationData;
	}

	/// <inheritdoc/>
	public async Task ResetPassword(CancellationToken ct, string email)
	{
		await _authenticationApiClient.ResetPassword(ct, email);
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
		var settings = await _applicationSettingsRepository.GetAndObserveCurrent().FirstAsync(ct);

		return settings.AuthenticationData;
	}

	private async Task NotifySessionExpiredInternal(CancellationToken ct, HttpRequestMessage request, AuthenticationData unauthorizedToken)
	{
		await _applicationSettingsRepository.SetAuthenticationData(ct, default(AuthenticationData));

		_sessionExpired.OnNext(Unit.Default);
	}

	private async Task<AuthenticationData> RefreshTokenInternal(CancellationToken ct, HttpRequestMessage request, AuthenticationData unauthorizedToken)
	{
		var authenticationData = await _authenticationApiClient.RefreshToken(ct, unauthorizedToken);

		await _applicationSettingsRepository.SetAuthenticationData(ct, authenticationData);

		return authenticationData;
	}
}
