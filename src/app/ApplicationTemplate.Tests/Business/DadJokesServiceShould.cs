using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using ApplicationTemplate.Client;
using DynamicData;
using Moq;
using Xunit;

namespace ApplicationTemplate.Tests.Business;

public partial class DadJokesServiceShould
{
	//private readonly Mock<IApplicationSettingsService> _applicationSettingsService;
	//private readonly Mock<IDadJokesEndpoint> _dadJokesEndpoint;
	//private readonly Mock<DadJokesService> _dadJokesService;
	//private SourceList<DadJokesQuote> _favouriteQuotes;
	//private ReplaySubject<PostTypes> _postType;

	//public DadJokesServiceShould()
	//{
	//	_applicationSettingsService = new Mock<IApplicationSettingsService>();
	//	_dadJokesEndpoint = new Mock<IDadJokesEndpoint>();
	//	_dadJokesService = new Mock<DadJokesService>();
	//	_postType = new ReplaySubject<PostTypes>(1);
	//	_postType.OnNext(PostTypes.Hot);
	//}

	[Fact]
	public async Task GetAllData()
	{
		var mockedDadJokesEndpoint = new Mock<IDadJokesEndpoint>();
		mockedDadJokesEndpoint
			.Setup(endpoint => endpoint.FetchData(It.IsAny<CancellationToken>(), "hot"))
			.ReturnsAsync();
	}
}
