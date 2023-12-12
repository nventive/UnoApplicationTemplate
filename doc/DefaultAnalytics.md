# Default Analytics

This application comes with a few default tracked events.
They can be found in the [IAnalyticsSink](../src/app/ApplicationTemplate.Presentation/Framework/Analytics/IAnalyticsSink.cs) interface.
The idea is that you would change the implementation of this interface to send the events to an analytics service (such as AppCenter, Firebase, Segment, etc.).

> 💡 The default events are meant to be a starting point for your application's analytics. Because they are automatic, they are more generic than custom events. If you want to track more specific events, you can adjust this recipe by adding new members to the `IAnalyticsSink` interface (or changing the existing ones) to better suit your needs.

Here is a list of the default events:

## Page Views
This is based on the changes of state from the `ISectionsNavigator`.

The `ISectionsNavigator` controls the navigation of the application. It's state can be observed and this is leveraged to detect the page views.

## Command Executions
This is based on the default builder of the `IDynamicCommandBuilderFactory`.

The `IDynamicCommandBuilder` allows to customize the default behavior or all DynamicCommands. This is leveraged to inject analytics on command invocations.

Command executions are typically associated with button presses and gestures.