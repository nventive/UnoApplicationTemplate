using System;
using ApplicationTemplate.Presentation;
using Nventive.View.Converters;

namespace ApplicationTemplate.Views
{
	public class FromPasswordStateToCustomValueConverter : ConverterBase
	{
		public object DefaultValue { get; set; }

		public object ValidValue { get; set; }

		public object InvalidValue { get; set; }

		protected override object Convert(object value, Type targetType, object parameter)
		{
			if (value is PasswordState passwordState)
			{
				switch (passwordState)
				{
					case PasswordState.Unedited:
						return DefaultValue;
					case PasswordState.Invalid:
						return InvalidValue;
					case PasswordState.Valid:
						return ValidValue;
				}
			}

			return DefaultValue;
		}
	}
}
