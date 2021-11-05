using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Allows to register a service into the dependency injection service collection.
	/// By default, the <see cref="RegistrationModes.Interface"/> registration mode is used with a <see cref="ServiceLifetime.Transient"/> lifetime.
	/// </summary>
	/// <param name="modes">The registration modes.</param>
	/// <param name="lifetime">The registration lifetime.</param>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class RegisterServiceAttribute : Attribute
	{
		public RegisterServiceAttribute(
			RegistrationModes modes = RegistrationModes.Interface,
			ServiceLifetime lifetime = ServiceLifetime.Transient)
		{
			Modes = modes;
			Lifetime = lifetime;
		}

		public RegisterServiceAttribute(ServiceLifetime lifetime, RegistrationModes modes = RegistrationModes.Interface)
			: this(modes, lifetime)
		{
		}

		public RegistrationModes Modes { get; }

		public ServiceLifetime Lifetime { get; }
	}
}
