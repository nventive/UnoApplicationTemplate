using System;
using ApplicationTemplate.Views.Controls;
using Nventive.View.Converters;

namespace ApplicationTemplate.Views;

public sealed class FromNullableAppBarButtonToAppBarButtonConverter : ConverterBase
{
	protected override object Convert(object value, Type targetType, object parameter)
	{
		return value is null ? new AppBarBackButton() : value;
	}
}
