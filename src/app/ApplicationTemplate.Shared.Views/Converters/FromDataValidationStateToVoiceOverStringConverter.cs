using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nventive.View.Converters;

namespace ApplicationTemplate.Views;

public sealed class FromDataValidationStateToVoiceOverStringConverter : ConverterBase
{
	protected override object Convert(object value, Type targetType, object parameter)
	{
		var validationFailures = value as DataValidationState;

		if (validationFailures != null && validationFailures.Errors.Any())
		{
			var readableString = string.Join(" \n", validationFailures.Errors);

			return readableString;
		}
		return null;
	}
}
