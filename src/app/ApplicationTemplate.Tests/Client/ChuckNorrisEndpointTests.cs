using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace ApplicationTemplate.Tests
{
	public class ChuckNorrisEndpointTests : TestBase<IChuckNorrisEndpoint>
	{
		[Fact]
		public async Task It_Should_GetAll()
		{
			var result = await SUT.Search(DefaultCancellationToken, "test");

			result.Should().NotBeNull();
			result.Quotes.Should().NotBeNullOrEmpty();
		}
	}
}
