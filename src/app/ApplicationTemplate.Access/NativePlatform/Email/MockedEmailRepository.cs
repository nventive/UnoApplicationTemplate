using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.DataAccess;

public sealed class MockedEmailRepository : IEmailRepository
{
	private readonly ILogger<MockedEmailRepository> _logger;

	public MockedEmailRepository(ILogger<MockedEmailRepository> logger)
	{
		_logger = logger;
	}

	public Task Compose(Email email)
	{
		_logger.LogInformation("Email composed: {Email}", email);
		return Task.CompletedTask;
	}
}
