using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;
using Uno;

namespace ApplicationTemplate.Presentation;

public sealed class EnvironmentPickerPageViewModel : ViewModel
{
	private readonly string _currentEnvironment;

	private readonly IEnvironmentManager _environmentManager;

	public EnvironmentPickerPageViewModel(string currentEnvironment)
	{
		ResolveService(out _environmentManager);
		_currentEnvironment = currentEnvironment;
	}

	public string SelectedEnvironment
	{
		get => this.Get(_currentEnvironment);
		set => this.Set(value);
	}

	public bool RequiresRestart
	{
		get => this.Get<bool>();
		private set => this.Set(value);
	}

	public IEnumerable<string> Environments => _environmentManager.AvailableEnvironments;

	public IDynamicCommand SelectEnvironment => this.GetCommandFromTask<string>(async (ct, environment) =>
	{
		if (_currentEnvironment == environment)
		{
			await this.GetService<ISectionsNavigator>().NavigateBack(ct);

			return;
		}

		_environmentManager.Override(environment);

		RequiresRestart = true;
	});
}
