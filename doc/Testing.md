# Testing

For more documentation on testing, read the references listed at the bottom.

## Unit testing

- We use [xUnit](https://www.nuget.org/packages/xunit/) to create the tests.
  You create a test as a `Fact` like this:

    ```csharp
    [Fact]
    public async Task It_Should_Do_Something()
    {
        ...
    }
    ```

- We use [FluentAssertions](https://www.nuget.org/packages/FluentAssertions/) to assert the result of a test. You assert the result of a test like this:

    ```csharp
    result.Should().NotBeNull();
    result.Id.Should().BeGreaterThan(0);
    result.Title.Should().Be(post.Title);
    result.Body.Should().Be(post.Body);
    result.UserIdentifier.Should().Be(post.UserIdentifier);
    ```

- We use [Moq](https://www.nuget.org/packages/Moq/) to mock behaviors. An example of a mocked object could look like this:

    ```csharp
    var mock = new Mock<IService>();
    mock.Setup(m => m.MyMethod("parameter")).Returns(true);

    var myService = mock.Object;
    myService.MyMethod("parameter");

    mock.Verify(m => m.MyMethod("parameter"), Times.AtMostOnce());
    ```

## Integration testing

- This template is unit-test safe which means we can do full integration tests using unit tests libraries without worrying about the user interface. This means you could do a unit-test that does the following:
  - Starts the application
  - Navigates to a specific page
  - Executes a command on that page
  - Executes an API call
  - Assert that the result is in the correct format and is cached in the app settings.

- This template provides a `TestBase` class that allows you to do those kinds of tests without worrying about bootstrapping your application; it does it for you. This is an example of an integration test.
  ```csharp
  public class MyIntegrationTest : TestBase
  {
      [Fact]
	  public async Task It_Should_Do_Something()
	  {
          var navigationService = GetService<IStackNavigator>();

          // From the home page, navigate to another page.
          var homeViewModel = (HomeViewModel)navigationService.State.Stack.Last().ViewModel;
          await homeViewModel.NavigateToOtherPage.Execute();

          // From the other page, execute an API request
          var otherViewModel = (OtherViewModel)navigationService.State.Stack.Last().ViewModel;
          await otherViewModel.GetFavorites.Execute();

          // Confirm the result is cached
          var settingsService = GetService<IApplicationSettingsService>();
          var favorites = settingService.GetFavorites();
          favorites.Should().NotBeEmpty();
      }
  }
  ```

  With a simple test, you can do very advanced integration tests. The `TestBase` class bootstraps the application in its initialization phase using the `CoreStartup`, reusing all the IoC already configured in your project.

### Mocking

You can also mock services that are normally registered with their implementations. In your `TestBase` class, simply override the `ConfigureHost` method and call the `ReplaceWithMock` method.

```csharp
  public class MyIntegrationTest : TestBase
  {
    protected override void ConfigureHost(IHostBuilder host)
    {
      host.ConfigureServices(s =>
      {
        // This will replace the actual implementation of IApplicationSettingsService with a mocked version.
        ReplaceWithMock<IApplicationSettingsService>(s, mock =>
        {
          ApplicationSettings result = ApplicationSettings.Default.WithIsOnboardingCompleted(false);
          mock.Setup(m => m.GetCurrent(AnyCancellationToken)).Returns(Task.FromResult(result));
          mock.Setup(m => m.GetAndObserveCurrent()).Returns(Observable.Return(result));
        });
      });
    }

    ...
  }
  ```

## Code coverage

We use [Coverlet.MSBuild](https://www.nuget.org/packages/coverlet.msbuild/) to collect code coverage data.

The result of the code coverage data (using the cobertura format) is used to generate a report that is presented as part of the CI process.

You can collect the code coverage locally using the following command line.

```
dotnet test .\src\ApplicationTemplate.sln /p:Configuration=Release_Tests /p:CollectCoverage=true /p:IncludeTestAssembly=true /p:CoverletOutputFormat=cobertura /p:ExcludeByFile="**/*.g.cs" /p:Exclude="[*]*.Tests.*" --logger trx --no-build
```

_Note: We need to include the test assembly since we're using shared projects. In order to ignore coverage on the tests themselves, we simply exclude anything under the `*.Tests` namespace._

## UI testing

- We use [Uno.UITest](https://github.com/unoplatform/Uno.UITest) and [Xamarin.UITest](https://docs.microsoft.com/en-us/appcenter/test-cloud/frameworks/uitest/) to execute UI tests.

- We use [SpecFlow](https://specflow.org/) to define the different UI tests to execute.

### References

- [Getting started with xUnit](https://xunit.net/docs/getting-started/netfx/visual-studio)
- [Getting started with Fluent Assertions](https://fluentassertions.com/introduction)
- [How to use Moq](https://github.com/moq/moq4)
- [How to use Coverlet](https://github.com/coverlet-coverage/coverlet)