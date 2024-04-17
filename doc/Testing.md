# Testing

For more documentation on testing, read the references listed at the bottom.

## Unit Testing

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

- We use [NSubstitute](https://www.nuget.org/packages/NSubstitute/) to mock behaviors. An example of a mocked object could look like this:

    ```csharp
    // Arrange
    var service = Substitute.For<IService>();
    service.MyMethod("parameter").Returns(true);

    // Act
    mock.MyMethod("parameter");
    
    // Assert
    mock.Received(1).MyMethod("parameter");
    ```

## Functional Testing

This template comes with full support for functional testing without any UI.
You can run simulations of the app with simple code and test complex scenarios.
Functional tests can be configured to use mocked or real API connections.

This template provides a `FunctionalTestBase` class that bootstraps the application with all its services and configurations.
This is an example of a functional test.

```csharp
public sealed class DadJokesShould : FunctionalTestBase
{
  [Fact]
  public async Task LoadAListOfJokes()
  {
    // Arrange

    // Start the app, reach the login page (by completing the onboarding), and login.
    await this.Login(await this.ReachLoginPage());

    // Get the active view model.
    var vm = GetAndAssertActiveViewModel<DadJokesPageViewModel>();

    // Act

    // Load the jokes. (That's normally done by the UI.)
    var jokes = await vm.Jokes.Load(CancellationToken.None);

    // Assert
    jokes.Should().NotBeEmpty();
  }
}
```

### Manipulating the App Simulation
The `FunctionalTestBase` class provides a few members to ease manipulating the app simulation.
- `ActiveViewModel` is the active view model returned by the `ISectionsNavigator`.
- `GetAndAssertActiveViewModel<T>` returns `ActiveViewModel` as the expected type `T`.
It throws an exception if the active view model is not of the expected type.
- `Shell` is the `ShellViewModel` of the app.
- `Menu` is the `MenuViewModel` of the app.
Use this to change sections as you would with the bottom navigation menu.
To better simulate the app behavior, accessing this property will fail when the active view model is not supposed to have the bottom menu.

On top of that, we suggest you extract common operations as extensions methods in `FunctionalTestBase.Extensions.cs` like we did for `ReachLoginPage` and `Login`.
This keeps the tests clean and easy to read.

### Overriding the Configuration
The `FunctionalTestBase` class has a few `virtual` members that you can override to change the app behavior for a specific test.

#### ConfigureHost
You can override the `ConfigureHost` method to add extra configuration.
This extra configuration is added on top of what `CoreStartup` does, which allows overriding the default configuration. 

```csharp
public sealed class DadJokesShould : FunctionalTestBase
{
  protected override void ConfigureHost(IHostBuilder hostBuilder)
  {
    // Use a custom implementation of IDadJokesRepository.
    hostBuilder.ConfigureServices(serviceCollection =>
      serviceCollection.AddSingleton<IDadJokesRepository, CustomDadJokesImplementation>()
    );
  }
}
```

#### ApplicationSettings

You can also override the `ApplicationSettings` property to change the settings of the app. This offers a simple way to change the behavior of the app that rely on settings without too much code.

```csharp
public override ApplicationSettings ApplicationSettings { get; } = new ApplicationSettings
{
  IsOnboardingCompleted = true,
};
```

## Logging

See https://xunit.net/docs/capturing-output.

When doing functional tests, you can pass the `ITestOutputHelper` to the base constructor of `FunctionalTestBase` to automatically setup the Serilog logging for your tests using the same configuration as the app.

## Naming

It is important to follow certain rules about the names of your class and your methods. The idea here is to make a sentence when combining your class name with one of your test.

- The suggested test class nomenclature is `"<TestedClass>Should"`.
- The suggested test method nomenclature is `"<ExpectedResult>_<Condition>"` (Condition is optional in the default case).

Here is an example:
Let's say we want to test this class.

```csharp
public class MyTestClassViewModel
{
  public async Task<int[]> MyTestMethod(bool isNeeded)
  {
    if (isNeeded)
    {
      return Array.Empty<int>();
    }
    ...

    return aFullArray;
  }

  // ...
}
```

 The test class of MyTestClassViewModel should look like this.
  
```csharp
public class MyTestClassViewModelShould
{
  [Fact]
  public async Task ReturnAnEmptyArray_WhenItIsNotNeeded()
  {
    // Arrange
    var vm = new MyTestClassViewModel()

    // Act
    var result = vm.MyTestMethod(false);

    // Assert
    result.Should().BeEmpty();
  }

  [Fact]
  public async Task ReturnAFullArray()
  {
    // Arrange
    var vm = new MyTestClassViewModel()

    // Act
    var result = vm.MyTestMethod(true);

    // Assert
    result.Should().NotBeEmpty();
  }

  // ...
}
```
  
 When executing this test class, the result will look something like this:
 - MyTestClassViewModelShould ReturnAFullArray -> My Test Class View Model Should Return A Full Array.
 - MyTestClassViewModelShould ReturnAnEmptyArray_WhenItIsNotNeeded -> My Test Class View Model Should Return An Empty Array When It Is Not Needed.

## Code coverage

We use [Coverlet.MSBuild](https://www.nuget.org/packages/coverlet.msbuild/) to collect code coverage data.

The result of the code coverage data (using the cobertura format) is used to generate a report that is presented as part of the CI process.

You can collect the code coverage locally using the following command lines.

- For Functional tests:
  ```powershell
  dotnet test src/app/ApplicationTemplate.Tests.Functional/ApplicationTemplate.Tests.Functional.csproj --collect:"XPlat Code Coverage" --settings build/test.runsettings
  ```

- For Unit tests:
  ```powershell
  dotnet test src/app/ApplicationTemplate.Tests.Unit/ApplicationTemplate.Tests.Unit.csproj --collect:"XPlat Code Coverage" --settings build/test.runsettings
  ```

### Limitations
There are some limitations related to code coverage.

- The **branch coverage** is not 100% accurate because we merge multiple reports into one.
See https://github.com/danielpalme/ReportGenerator/issues/455.

## References

- [Getting started with xUnit](https://xunit.net/docs/getting-started/netfx/visual-studio)
- [Getting started with Fluent Assertions](https://fluentassertions.com/introduction)
- [How to use NSubstitute](https://github.com/nsubstitute/NSubstitute)
- [How to use Coverlet](https://github.com/coverlet-coverage/coverlet)
