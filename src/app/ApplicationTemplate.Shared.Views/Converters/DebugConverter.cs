using System;
using System.Collections.Generic;
using System.Text;
using Nventive.View.Converters;

namespace ApplicationTemplate
{
	/// <summary>
	/// Use this converter to debug data bindings in your xaml.
	/// </summary>
	public class DebugConverter : ConverterBase
	{
		protected override object Convert(object value, Type targetType, object parameter)
		{
			// Put a breakpoint here to inspect values from the ViewModel to the View.
			return value;
		}

		protected override object ConvertBack(object value, Type targetType, object parameter)
		{
			// Put a breakpoint here to inspect values from the View to the ViewModel.
			return value;
		}
	}
}
