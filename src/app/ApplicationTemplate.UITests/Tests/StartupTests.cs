using NUnit.Framework;

namespace ApplicationTemplate.UITests.Tests;

[TestFixture]
public class StartupTests : TestBase
{
	[Test]
	public void ShowRepl()
	{
		TestBase.App.Repl();
	}
}
