using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.DataAccess.PlatformServices;

public sealed class MockedEmailService : IEmailService
{
	private readonly ILogger<MockedEmailService> _logger;

	public MockedEmailService(ILogger<MockedEmailService> logger)
	{
		_logger = logger;
	}

	public Task Compose(Email email)
	{
		_logger.LogInformation("Email composed: {Email}", email);
		return Task.CompletedTask;
	}
}
