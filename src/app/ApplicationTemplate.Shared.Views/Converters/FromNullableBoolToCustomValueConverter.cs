using System;
using ApplicationTemplate.Presentation;
using Nventive.View.Converters;

namespace ApplicationTemplate.Views
{
	public class FromNullableBoolToCustomValueConverter : ConverterBase
	{
		public object DefaultValue { get; set; }

		public object ValidValue { get; set; }

		public object InvalidValue { get; set; }

		protected override object Convert(object value, Type targetType, object parameter)
		{
			if (value is bool?)
			{
				switch (value)
				{
					case null:
						return DefaultValue;
					case false:
						return InvalidValue;
					case true:
						return ValidValue;
				}
			}

			return DefaultValue;
		}
	}
}
