using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationTemplate;

/// <summary>
/// Contains the configuration options of everything related to data mocking.
/// </summary>
public class MockOptions
{
	/// <summary>
	/// Gets or sets a value indicating whether mock implementations should be used.
	/// </summary>
	public bool IsMockEnabled { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether a delay should be added to simulate API calls.
	/// </summary>
	public bool IsDelayForSimulatedApiCallsEnabled { get; set; }

	// Feel free to add more specific properties in this class if you want a more granular control on mock implementations and their behavior.
	// Here are some examples:
	// You could use enums to define mock presets.
	// You could have one property per API client.
	// You could have multiple implementations for the same mocked service.
	// You could have custom input for mocked services to control their behavior.
}
