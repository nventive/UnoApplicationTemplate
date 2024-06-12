using System;
using ApplicationTemplate.Views.Controls;
using Nventive.View.Converters;

namespace ApplicationTemplate.Views;

/// <summary>
/// If command bar isn't already using <see cref="Uno.UI.Toolkit.CommandBarExtensions.NavigationCommandProperty"/> return <see cref="AppBarBackButton"/>.
/// </summary>
public sealed class FromNullableAppBarButtonToAppBarButtonConverter : ConverterBase
{
	protected override object Convert(object value, Type targetType, object parameter)
	{
		return value is null ? new AppBarBackButton() : value;
	}
}
