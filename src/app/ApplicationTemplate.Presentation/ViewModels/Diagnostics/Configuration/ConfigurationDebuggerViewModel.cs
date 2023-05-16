using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Text.Json;
using Chinook.DynamicMvvm;
using MessageDialogService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Uno;

namespace ApplicationTemplate.Presentation;

public sealed partial class ConfigurationDebuggerViewModel : TabViewModel
{
	private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
	{
		WriteIndented = true
	};

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "It will actually be disposed because we're using AddDisposable.")]
	private readonly SerialDisposable _changeCallback = new();

	[Inject] private IDiagnosticsService _diagnosticsService;

	public ConfigurationDebuggerViewModel()
	{
		Title = "Configuration";

		var config = this.GetService<IConfiguration>();
		UpdateJson(config);

		_changeCallback.Disposable = config
			.GetReloadToken()
			.RegisterChangeCallback(OnConfigurationChanged, config);

		AddDisposable(nameof(_changeCallback), _changeCallback);
	}

	private void OnConfigurationChanged(object configuration)
	{
		var config = (IConfiguration)configuration;
		UpdateJson(config);

		// We need to re-register the callback because it's a one-time callback.
		_changeCallback.Disposable = config
			.GetReloadToken()
			.RegisterChangeCallback(OnConfigurationChanged, config);
	}

	public IDynamicCommand DeleteConfigurationOverride => this.GetCommandFromTask(async ct =>
	{
		_diagnosticsService.DeleteConfigurationOverrideFile();

		await this.GetService<IMessageDialogService>().ShowMessage(ct, mb => mb
			.Title("Diagnostics")
			.Content("The configuration override was deleted. Restart the application to see your changes.")
			.OkCommand()
		);
	});

	/// <summary>
	/// Gets the ­­JSON representation of the configuration.
	/// The JSON has its full hierarchy to look as close as possible to appsettings.json content.
	/// </summary>
	public string ConfigurationAsJson
	{
		get => this.Get(initialValue: default(string));
		private set => this.Set(value);
	}

	private void UpdateJson(IConfiguration config)
	{
		var json = ToJsonString(config);
		ConfigurationAsJson = json.ToString();
	}

	private static string ToJsonString(IConfiguration configuration)
	{
		var result = new Dictionary<string, object>();
		foreach (var child in configuration.GetChildren())
		{
			result[child.Key] = GetConfigurationValue(child);
		}
		return JsonSerializer.Serialize(result, _jsonOptions);
	}

	private static object GetConfigurationValue(IConfigurationSection section)
	{
		var children = section.GetChildren();
		if (children.Any())
		{
			var result = new Dictionary<string, object>();
			foreach (var child in children)
			{
				result[child.Key] = GetConfigurationValue(child);
			}
			return result;
		}
		else
		{
			return section.Value switch
			{
				"true" or "True" => true,
				"false" or "False" => false,
				_ => section.Value,
			};
		}
	}
}
