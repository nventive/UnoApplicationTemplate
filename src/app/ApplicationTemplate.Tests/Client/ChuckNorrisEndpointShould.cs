using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace ApplicationTemplate.Tests
{
	public class ChuckNorrisEndpointShould : IntegrationTestBase<IChuckNorrisEndpoint>
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
