using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Chinook.DynamicMvvm;
using MessageDialogService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
	private readonly string[] _initialKeys;

	[Inject] private IDiagnosticsService _diagnosticsService;
	[Inject] private IEnvironmentManager _environmentManager;

	public ConfigurationDebuggerViewModel()
	{
		Title = "Configuration";
		AvailableEnvironments = _environmentManager.AvailableEnvironments;
		ResetEnvironmentContent = $"Reset to default ({_environmentManager.Default})";
		CurrentEnvironment = _environmentManager.Current;

		_initialKeys = GetAllKeys();

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
	/// Gets all possible values for the <see cref="SelectedKey"/> property.
	/// </summary>
	public string[] AllKeys
	{
		get => this.Get(initialValue: _initialKeys);
		private set => this.Set(value);
	}

	public string SelectedKey
	{
		get => this.Get(initialValue: default(string));
		set
		{
			ConfigurationKey = value;
			ConfigurationValue = this.GetService<IConfiguration>()[value];

			// Erase the selection from the ComboBox because we put it in the TextBox.
			Task.Run(() => RaisePropertyChanged(nameof(SelectedKey)));
		}
	}

	public string ConfigurationKey
	{
		get => this.Get(initialValue: string.Empty);
		set => this.Set(value);
	}

	public string ConfigurationValue
	{
		get => this.Get(initialValue: string.Empty);
		set => this.Set(value);
	}

	public bool CanSave => this.GetFromDynamicProperty(
		source: this.GetProperty(x => x.ConfigurationKey),
		selector: key => !string.IsNullOrEmpty(key)
	);

	public IDynamicCommand Save => this.GetCommand(() =>
		{
			var config = this.GetService<IConfiguration>();
			config[ConfigurationKey] = ConfigurationValue;

			// Clear the input value, but keep the input key.
			ConfigurationValue = string.Empty;

			// Add the new key to the list if necessary.
			if (!AllKeys.Contains(ConfigurationKey))
			{
				AllKeys = GetAllKeys();
			}
		},
		c => c.WithCanExecute(this.GetProperty(x => x.CanSave))
	);

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

	private string[] GetAllKeys()
	{
		var ignoredPrefixes = GetIgnoredPrefixes().ToArray();

		return Enumerable
			.Union(
				GetKeysFromConfiguration(), // We get what is currently in the IConfiguration.
				GetKeysFromOptionsTypes() // We add what might be missing from the IConfiguration (because default values aren't shown in the IConfiguration).
			)
			.Where(key => !ignoredPrefixes.Any(key.StartsWith))
			.OrderBy(key => key)
			.ToArray();

		static IEnumerable<string> GetIgnoredPrefixes()
		{
			yield return "contentRoot";
			yield return "Environment";
			yield return ApplicationTemplateConfigurationExtensions.DefaultOptionsName<ReadOnlyConfigurationOptions>();
		}
	}

	private static List<string> GetKeysFromOptionsTypes()
	{
		var optionsTypes = GetAssemblies().SelectMany(a => a.GetTypes().Where(t => t.Name.EndsWith("Options") && !t.IsAbstract && t.IsClass)).ToArray();
		var keys = new List<string>();

		foreach (var optionsType in optionsTypes)
		{
			GenerateKeysFromType(optionsType, keys);
		}

		return keys;

		IEnumerable<Assembly> GetAssemblies()
		{
			// You could add more assemblies if you want to scan for more "Options" suffixed types.
			yield return Assembly.GetAssembly(typeof(SerializationConfiguration)); // Access assembly
			yield return Assembly.GetExecutingAssembly(); // Presentation assembly
		}

		static void GenerateKeysFromType(Type type, List<string> keys, string parentKey = null)
		{
			foreach (var property in type.GetProperties())
			{
				parentKey ??= ApplicationTemplateConfigurationExtensions.DefaultOptionsName(type);
				var key = $"{parentKey}:{property.Name}";
				var propertyType = property.PropertyType;

				if (!propertyType.IsPrimitive
					&& propertyType != typeof(string)
					&& propertyType != typeof(Uri)
					&& propertyType != typeof(TimeSpan)
				)
				{
					GenerateKeysFromType(property.PropertyType, keys, key);
				}
				else
				{
					keys.Add(key);
				}
			}
		}
	}

	private List<string> GetKeysFromConfiguration()
	{
		var configuration = this.GetService<IConfiguration>();
		var keys = new List<string>();
		ExtractKeys(configuration, keys);
		RemoveTopLevelKeys(keys);
		return keys;

		// Extract keys from the configuration.
		static void ExtractKeys(IConfiguration configuration, List<string> keys, string parentKey = null)
		{
			foreach (var child in configuration.GetChildren())
			{
				var currentKey = parentKey is null
					? child.Key
					: $"{parentKey}:{child.Key}";

				keys.Add(currentKey);
				ExtractKeys(child, keys, currentKey);
			}
		}

		// Removes keys that represent container objects.
		static void RemoveTopLevelKeys(List<string> keys)
		{
			for (int i = 0; i < keys.Count; i++)
			{
				var current = keys[i];

				for (int j = i + 1; j < keys.Count; j++)
				{
					var next = keys[j];
					if (next.StartsWith(current))
					{
						keys.RemoveAt(i);
						--i;
						break;
					}
				}
			}
		}
	}
}
