using System;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using ApplicationTemplate.DataAccess;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Uno.Extensions;
using Xunit;

namespace ApplicationTemplate.Tests;

/// <summary>
/// Tests for the <see cref="AuthenticationService"/>.
/// </summary>
public sealed class AuthenticationServiceShould : IDisposable
{
	private readonly IAuthenticationService _authenticationService;
	private readonly IApplicationSettingsRepository _applicationSettingsRepository;
	private readonly IAuthenticationApiClient _authenticationApiClient;
	private readonly BehaviorSubject<ApplicationSettings> _applicationSettingsSubject;

	public AuthenticationServiceShould()
	{
		_applicationSettingsRepository = Substitute.For<IApplicationSettingsRepository>();
		_authenticationApiClient = Substitute.For<IAuthenticationApiClient>();

		_authenticationService = new AuthenticationService(
			Substitute.For<ILoggerFactory>(),
			_applicationSettingsRepository,
			_authenticationApiClient
		);

		_applicationSettingsSubject = new BehaviorSubject<ApplicationSettings>(CreateApplicationSettings());

		_applicationSettingsRepository.GetAndObserveCurrent().Returns(_applicationSettingsSubject);
		_applicationSettingsRepository.GetCurrent(CancellationToken.None).Returns(Task.FromResult(_applicationSettingsSubject.Value));

		_applicationSettingsRepository.When(x => x.SetAuthenticationData(Arg.Any<CancellationToken>(), Arg.Any<AuthenticationData>()))
			.Do(x => _applicationSettingsSubject.OnNext(CreateApplicationSettings(x.Arg<AuthenticationData>())));
	}

	[Fact]
	public async Task ReturnAuthenticationData_WhenLoggingIn()
	{
		// Arrange
		var email = "email";
		var password = "password";
		var authenticationData = CreateAuthenticationData(email: email);

		_authenticationApiClient.Login(Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>()).Returns(authenticationData);

		// Act
		var result = await _authenticationService.Login(CancellationToken.None, email, password);

		// Assert
		Assert.Equal(authenticationData.AccessToken, result.AccessToken);
	}

	[Fact]
	public async Task ReturnAuthenticationData_WhenRefreshingTheToken()
	{
		// Arrange
		var email = "email";
		var password = "password";
		var now = DateTimeOffset.Now;
		var authenticationData = CreateAuthenticationData(email: email, now: now);

		now = now.Add(TimeSpan.FromMinutes(1)); // Advance the time to simulate the token expiration.

		var refreshAuthenticationData = CreateAuthenticationData(email: email, now: now);

		_authenticationApiClient.Login(Arg.Any<CancellationToken>(), Arg.Any<string>(), Arg.Any<string>()).Returns(authenticationData);
		_authenticationApiClient.RefreshToken(Arg.Any<CancellationToken>(), Arg.Any<AuthenticationData>()).Returns(refreshAuthenticationData);

		// Act
		var loginResult = await _authenticationService.Login(CancellationToken.None, email, password);
		var result = await _authenticationService.RefreshToken(CancellationToken.None, new HttpRequestMessage(), loginResult);

		// Assert
		Assert.True(result.AccessToken.Payload.IssuedAt > loginResult.AccessToken.Payload.IssuedAt);
	}

	[Fact]
	public async Task NotifySuscribersThatTheSessionExpired_WhenNotifySesionExpiredIsCalled()
	{
		// Arrange
		var notifySessionExpired = false;

		_authenticationService.ObserveSessionExpired().Subscribe(_ => notifySessionExpired = true);

		// Act
		await _authenticationService.NotifySessionExpired(CancellationToken.None, new HttpRequestMessage(), CreateAuthenticationData("email"));

		// Assert
		Assert.True(notifySessionExpired);
	}

	[Fact]
	public async Task DiscardUserSettings_OnLogout()
	{
		// Arrange
		// Act
		await _authenticationService.Logout(CancellationToken.None);

		// Assert
		await _applicationSettingsRepository.Received().DiscardUserSettings(CancellationToken.None);
	}

	[Fact]
	public async Task SignalThatTheUserIsLoggedOut_WhenThereIsNoAuthenticationDataInAppSettings()
	{
		// Arrange
		ResetApplicationSettings();

		// Act
		var result = await _authenticationService.GetAndObserveIsAuthenticated().FirstAsync();

		// Assert
		Assert.False(result);
	}

	[Fact]
	public async Task ReturnNull_WhenThereIsNoAuthenticationDataInAppSettings()
	{
		// Arrange
		ResetApplicationSettings();

		// Act
		var result = await _authenticationService.GetAndObserveAuthenticationData().FirstAsync();

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public async Task SignalThatUserIsLoggedIn_WhenTheSettingsContainsAuthenticationData()
	{
		// Arrange
		var authenticationData = CreateAuthenticationData("email");

		_applicationSettingsSubject.OnNext(CreateApplicationSettings(authenticationData));

		// Act
		var result = await _authenticationService.GetAndObserveIsAuthenticated().FirstAsync();

		// Assert
		Assert.True(result);
	}

	[Fact]
	public async Task GiveAuthData_WhenTheSettingsContainsAuthenticationData()
	{
		// Arrange
		var authenticationData = CreateAuthenticationData("email");

		_applicationSettingsSubject.OnNext(CreateApplicationSettings(authenticationData));

		// Act
		var result = await _authenticationService.GetAndObserveAuthenticationData().FirstAsync();

		// Assert
		Assert.Equal(authenticationData.AccessToken, result.AccessToken);
	}

	public void Dispose()
	{
		_applicationSettingsSubject.Dispose();
	}

	private void ResetApplicationSettings()
	{
		_applicationSettingsSubject.OnNext(CreateApplicationSettings());
	}

	private AuthenticationData CreateAuthenticationData(string email, DateTimeOffset? now = null)
	{
		return new AuthenticationData
		{
			AccessToken = new JwtData<AuthenticationToken>(AuthenticationApiClientMock.CreateJsonWebToken(email: email, now: now, serializerOptions: SerializationConfiguration.DefaultJsonSerializerOptions)),
			RefreshToken = "RefreshToken",
		};
	}

	private ApplicationSettings CreateApplicationSettings(AuthenticationData authenticationData = null)
	{
		return new ApplicationSettings() { AuthenticationData = authenticationData };
	}
}
