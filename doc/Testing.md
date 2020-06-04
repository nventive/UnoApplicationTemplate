## Testing

We use [xUnit](https://www.nuget.org/packages/xunit/) to create the tests.

- You create a test as a `Fact` like this:

    ```csharp
    [Fact]
    public async Task ItShouldDoSomething()
    {
        ...
    }
    ```

We use [FluentAssertions](https://www.nuget.org/packages/FluentAssertions/) to assert the result of a test.

- You assert the result of a test like this:

    ```csharp
    result.Should().NotBeNull();
    result.Id.Should().BeGreaterThan(0);
    result.Title.Should().Be(post.Title);
    result.Body.Should().Be(post.Body);
    result.UserIdentifier.Should().Be(post.UserIdentifier);
    ```

We use [Moq](https://www.nuget.org/packages/Moq/) to mock behaviors.

- An example of a mocked object could look like this:

    ```csharp
    var mock = new Mock<IService>();
    mock.Setup(m => m.MyMethod("parameter")).Returns(true);

    var myService = mock.Object;
    myService.MyMethod("parameter");

    mock.Verify(m => m.MyMethod("parameter"), Times.AtMostOnce());
    ```

### References

- [Getting started with xUnit](https://xunit.net/docs/getting-started/netfx/visual-studio)
- [Getting started with Fluent Assertions](https://fluentassertions.com/introduction)
- [How to use Moq](https://github.com/moq/moq4)
