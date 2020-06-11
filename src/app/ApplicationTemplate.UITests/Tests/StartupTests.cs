using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ApplicationTemplate.UITests.Tests
{
	[TestFixture]
	public class StartupTests : TestBase
	{
		[Test]
		public void ShowRepl()
		{
			TestBase.App.Repl();
		}
	}
}
