using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Uno;

namespace Chinook.DataLoader;

/// <summary>
/// This class is a <see cref="DelegatingDataLoaderStrategy"/> that logs any load error.
/// This class demontrates how easy it is to extend the DataLoader recipe.
/// </summary>
public class LogErrorDataLoaderStrategy : DelegatingDataLoaderStrategy
{
	private readonly ILogger _logger;

	public LogErrorDataLoaderStrategy(ILogger logger)
	{
		_logger = logger;
	}

	public override async Task<object> Load(CancellationToken ct, IDataLoaderRequest request)
	{
		try
		{
			var result = await base.Load(ct, request);

			return result;
		}
		catch (Exception error)
		{
			_logger.LogError(error, "Failed to load request '{RequestSequenceId}' in DataLoader '{DataLoaderName}'.", request.SequenceId, request.Context.GetDataLoaderName());

			throw;
		}
	}
}

public static class LogErrorDataLoaderStrategyExtensions
{
	/// <summary>
	/// Adds a <see cref="LogErrorDataLoaderStrategy"/> to this builder.
	/// </summary>
	/// <typeparam name="TBuilder">The type of the builder.</typeparam>
	/// <param name="builder">The builder.</param>
	/// <param name="logger">The logger.</param>
	/// <returns>The original builder.</returns>
	public static TBuilder WithLoggedErrors<TBuilder>(this TBuilder builder, ILogger logger)
		where TBuilder : IDataLoaderBuilder
	{
		return builder.WithStrategy(new LogErrorDataLoaderStrategy(logger));
	}
}
