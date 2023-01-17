using System;
using System.Threading.Tasks;

namespace ApplicationTemplate;

public interface IBrowserService
{
	/// <summary>
	/// Opens the default application associated with the <see cref="Uri"/>.
	/// </summary>
	/// <param name="uri">The <see cref="Uri"/> to launch.</param>
	/// <returns><see cref="Task"/>.</returns>
	Task OpenAsync(Uri uri);
}
