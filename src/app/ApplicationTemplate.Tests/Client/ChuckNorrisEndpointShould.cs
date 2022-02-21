using System;
using System.Threading.Tasks;
using ApplicationTemplate.Client;
using FluentAssertions;
using Xunit;

namespace ApplicationTemplate.Tests
{
	public class DadJokesEndpointShould : IntegrationTestBase<IDadJokesEndpoint>
	{
		[Fact]
		public async Task GetAll()
		{
			// Act
			var result = await SUT.Search(DefaultCancellationToken, "test");

			// Assert
			result.Should().NotBeNull();
			result.Quotes.Should().NotBeNullOrEmpty();
		}
	}
}
