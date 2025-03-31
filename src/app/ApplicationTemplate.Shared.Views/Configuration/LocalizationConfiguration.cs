using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Windows.ApplicationModel.Resources;

namespace ApplicationTemplate.Views;

/// <summary>
/// This class is used for localization configuration.
/// - Configures the supported cultures.
/// - Configures the localization services.
/// </summary>
public static class LocalizationConfiguration
{
	private static ThreadCultureOverrideService _cultureOverrideService;

	private static CultureInfo[] SupportedCultures { get; } =
	[
		// TODO: Set your supported cultures here.
		new CultureInfo("en-CA"),
		new CultureInfo("fr-CA"),
	];

	/// <summary>
	/// Adds the localization services to the <see cref="IServiceCollection"/>.
	/// </summary>
	/// <param name="services">Service collection.</param>
	/// <returns><see cref="IServiceCollection"/>.</returns>
	public static IServiceCollection AddLocalization(this IServiceCollection services)
	{
		return services
			.AddSingleton(_cultureOverrideService)
			.AddSingleton<IThreadCultureOverrideService>(_cultureOverrideService)
			.AddSingleton<IStringLocalizer, ResourceLoaderStringLocalizer>();
	}

	public static void PreInitialize()
	{
		// This will override the system culture with a user preference.
		// This is used to change the language of the application.
		// It needs to be executed very early on the UI thread to make sure the app is completely localized.
		_cultureOverrideService = new ThreadCultureOverrideService(
			Thread.CurrentThread,
			SupportedCultures.Select(c => c.TwoLetterISOLanguageName).ToArray(),
			SupportedCultures.First(),
			GetSettingFilePath()
		);

		_cultureOverrideService.TryApply();
	}

	/// <summary>
	/// Gets the path to the culture override settings file.
	/// </summary>
	private static string GetSettingFilePath()
	{
//-:cnd:noEmit
#if __WINDOWS__
//+:cnd:noEmit
		var folderPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
//-:cnd:noEmit
#elif __ANDROID__ || __IOS__
//+:cnd:noEmit
		var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
//-:cnd:noEmit
#else
//+:cnd:noEmit
		var folderPath = string.Empty;
//-:cnd:noEmit
#endif
		//+:cnd:noEmit

		return Path.Combine(folderPath, "culture-override");
	}
}

/// <summary>
/// This implementation of <see cref="IStringLocalizer"/> uses <see cref="ResourceLoader"/>
/// to get the string resources.
/// </summary>
public class ResourceLoaderStringLocalizer : IStringLocalizer
{
	private const string SearchLocation = "Resources";
	private readonly ResourceLoader _resourceLoader;
	private readonly bool _treatEmptyAsNotFound;

	/// <summary>
	/// Initializes a new instance of the <see cref="ResourceLoaderStringLocalizer"/> class.
	/// </summary>
	/// <param name="treatEmptyAsNotFound">If empty strings should be treated as not found.</param>
	public ResourceLoaderStringLocalizer(bool treatEmptyAsNotFound = true)
	{
		_treatEmptyAsNotFound = treatEmptyAsNotFound;
		_resourceLoader = ResourceLoader.GetForViewIndependentUse();
	}

	/// <inheritdoc/>
	public LocalizedString this[string name] => GetLocalizedString(name);

	/// <inheritdoc/>
	public LocalizedString this[string name, params object[] arguments] => GetLocalizedString(name, arguments);

	private LocalizedString GetLocalizedString(string name, params object[] arguments)
	{
		ArgumentNullException.ThrowIfNull(name);

		var resource = _resourceLoader.GetString(name);

		if (_treatEmptyAsNotFound && string.IsNullOrEmpty(resource))
		{
			resource = null;
		}

		var resourceNotFound = resource is null;

		resource ??= name;

		var value = arguments.Length != 0
			? string.Format(CultureInfo.CurrentCulture, resource, arguments)
			: resource;

		return new LocalizedString(name, value, resourceNotFound: resourceNotFound, searchedLocation: SearchLocation);
	}

	/// <inheritdoc/>
	public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
		=> throw new NotSupportedException("ResourceLoader doesn't support listing all strings.");
}

public class ThreadCultureOverrideService : IThreadCultureOverrideService
{
	private readonly Thread _uiThread;
	private readonly string _settingFilePath;
	private readonly string[] _supportedLanguages;
	private readonly CultureInfo _fallbackCulture;
	private readonly ILogger _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="ThreadCultureOverrideService"/> class.
	/// </summary>
	/// <param name="uiThread">UI thread.</param>
	/// <param name="supportedLanguages">Supported languages.</param>
	/// <param name="fallbackCulture">Fallback culture.</param>
	/// <param name="settingFilePath">Path to the file where the preference will be stored.</param>
	/// <param name="logger">Logger.</param>
	public ThreadCultureOverrideService(
		Thread uiThread,
		string[] supportedLanguages,
		CultureInfo fallbackCulture,
		string settingFilePath,
		ILogger<ThreadCultureOverrideService> logger = null
	)
	{
		_uiThread = uiThread ?? throw new ArgumentNullException(nameof(uiThread));
		_supportedLanguages = supportedLanguages ?? throw new ArgumentNullException(nameof(supportedLanguages));
		_fallbackCulture = fallbackCulture ?? throw new ArgumentNullException(nameof(supportedLanguages));
		_settingFilePath = settingFilePath ?? throw new ArgumentNullException(nameof(settingFilePath));
		_logger = logger ?? NullLogger<ThreadCultureOverrideService>.Instance;
	}

	/// <inheritdoc/>
	public bool TryApply()
	{
		try
		{
//-:cnd:noEmit
#if ANDROID
//+:cnd:noEmit
			// We need a special case for Android because the CurrentCulture does not return the correct value after changing the device language.
			var locale = Java.Util.Locale.Default; // Gets the real system language
			var culture = new CultureInfo(locale.ToString());
//-:cnd:noEmit
#else
//+:cnd:noEmit
			var culture = CultureInfo.CurrentCulture;
//-:cnd:noEmit
#endif
//+:cnd:noEmit

			// Use the settings culture if it is set.
			var settingsCulture = GetCulture();
			if (settingsCulture != null)
			{
				culture = settingsCulture;
			}

			// Use the fallback culture if the language is not supported.
			if (!_supportedLanguages.Any(l => l.StartsWith(culture.TwoLetterISOLanguageName, StringComparison.InvariantCultureIgnoreCase)))
			{
				culture = _fallbackCulture;
			}

			// Set the language override. This is necessary on WinUI for the ResourceLoader to work.
			Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = culture.TwoLetterISOLanguageName;

			// Override the current thread culture.
			_uiThread.CurrentCulture = culture;
			_uiThread.CurrentUICulture = culture;

			// Override any new thread culture.
			CultureInfo.DefaultThreadCurrentCulture = culture;
			CultureInfo.DefaultThreadCurrentUICulture = culture;

			return true;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to apply the culture override.");
			return false;
		}
	}

	/// <inheritdoc/>
	public CultureInfo GetCulture()
	{
		if (!File.Exists(_settingFilePath))
		{
			return null;
		}

		var culture = File.ReadAllText(_settingFilePath);

		return new CultureInfo(culture);
	}

	/// <inheritdoc/>
	public void SetCulture(CultureInfo culture)
	{
		ArgumentNullException.ThrowIfNull(culture);

		using var writer = File.CreateText(_settingFilePath);
		writer.Write(culture.Name);
	}
}
