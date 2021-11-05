using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
	[Flags]
	public enum RegistrationModes
	{
		/// <summary>
		/// Allows to register the interface.
		/// </summary>
		Interface,

		/// <summary>
		/// Allows to register the concrete class.
		/// </summary>
		ConcreteClass,
	}
}
