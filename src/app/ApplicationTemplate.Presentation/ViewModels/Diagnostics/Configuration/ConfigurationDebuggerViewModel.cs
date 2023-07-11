using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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
	[Inject] private IEnvironmentManager _environmentManager;

	public ConfigurationDebuggerViewModel()
	{
		Title = "Configuration";
		AvailableEnvironments = _environmentManager.AvailableEnvironments;
		ResetEnvironmentContent = $"Reset to default ({_environmentManager.Default})";
		CurrentEnvironment = _environmentManager.Current;

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

	public IDynamicCommand ResetEnvironment => this.GetCommandFromTask(async ct =>
	{
		_environmentManager.ClearOverride();
		NextEnvironment = _environmentManager.Next;

		// We set the property using the Set method instead of directly using the property to avoid invoking OnSelectedEnvironmentChanged.
		this.Set(_environmentManager.Default, nameof(SelectedEnvironment));
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

	public string CurrentEnvironment { get; }

	public string NextEnvironment
	{
		get => this.Get(initialValue: _environmentManager.Next);
		private set => this.Set(value);
	}

	public bool IsNextEnvironmentDifferentThanCurrent => this.GetFromDynamicProperty(
		source: this.GetProperty(x => x.NextEnvironment),
		selector: next => next != CurrentEnvironment
	);

	public string ResetEnvironmentContent { get; }

	public string[] AvailableEnvironments { get; }

	public string SelectedEnvironment
	{
		get => this.Get(initialValue: _environmentManager.Current);
		set
		{
			this.Set(value);
			_ = Task.Run(OnSelectedEnvironmentChanged);
		}
	}

	private void OnSelectedEnvironmentChanged()
	{
		_environmentManager.Override(SelectedEnvironment);
		NextEnvironment = _environmentManager.Next;
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
