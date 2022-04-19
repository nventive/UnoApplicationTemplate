using System;
using Nventive.View.Converters;

namespace ApplicationTemplate.Views
{
	/// <summary>
	/// Use this converter to convert a string .
	/// </summary>
	public class FiltersConverter : ConverterBase
	{
		protected override object Convert(object value, Type targetType, object parameter)
		{
			return 1 == 1;
		}

		protected override object ConvertBack(object value, Type targetType, object parameter)
		{
			return (bool)value ? parameter : null;
		}
	}
}
