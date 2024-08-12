# Diagnostic Tools

This template comes with multiple built-in diagnostic tools.

## Diagnostics Overlay
When you start the application in debug, you'll notice a box on the side of the screen.
This overlay shows various counters and buttons.
The overlay is accessible from anywhere in your app.
This is useful when you want to see something happening live.

![Diagnostics-Overlay-Preview](https://user-images.githubusercontent.com/39710855/264691340-dbc9d137-a199-4969-94d7-7dd430e08da7.gif)

**Feel free to add more counters and buttons to ease your work!**

The default counters track the amount of active and uncollected ViewModel, DynamicProperties, and DynamicCommands.
The default buttons include commands such as the following.
- **Collect** runs the garbage collector.
- **Theme** toggles between the light and dark theme for the whole application.
- **Expand/Minimize** opens or closes the expanded view of the overlay.
- **Move** moves the overlay left or right.
- **X** hides the overlay.

## Diagnostics Page
The diagnostics page is a regular app page.
It's accessed by tapping multiple times on the application version.
This page contains information and configuration settings.
By default it shows things like the following.
- Build information
- Device information
- Startup performance
- Configuration values

The page also allows you to manually set some configuration values.
- Application language
- Logging options
- Mock data

It also has various diagnostics utilities such as the following.
- Send information via email (including log files as attachements).
- Throw exceptions from various threads (to test the error handling or crash reporting).
- Open the settings folder where the settings files can be found (WinUI only).

## Http Debugger

On the expanded diagnostics overlay, tap on the **HTTP** button to show the `HttpDebuggerView`.

![HTTP-Debugger-Screenshot](https://user-images.githubusercontent.com/39710855/264707239-2c9758ee-2d89-42a1-8843-58c3a85710fd.png)

This view shows all the http calls and their status.
You can click on items to see more details, such as headers, content, elapsed time, etc.
Check the following for more details.
- [`IHttpDebuggerService`](..\src\app\ApplicationTemplate.Access\Framework\HttpDebugger\IHttpDebuggerService.cs)
- [`HttpDebuggerHandler`](..\src\app\ApplicationTemplate.Access\Framework\HttpDebugger\HttpDebuggerHandler.cs)
- [`HttpDebuggerViewModel`](..\src\app\ApplicationTemplate.Presentation\ViewModels\Diagnostics\HttpDebugger\HttpDebuggerViewModel.cs)
- [`HttpDebuggerView`](..\src\app\ApplicationTemplate.Shared.Views\Content\Diagnostics\HttpDebuggerView.xaml)

## Configuration Debugger

On the expanded diagnostics overlay, tap on the **Configuration** button to show the `ConfigurationDebuggerView`.

![Configuration-Debugger-Screenshot](https://user-images.githubusercontent.com/39710855/264707102-bb020245-4d9b-4152-b72a-121344f42ec5.png)

This view shows a JSON representation of the configuration.

It also allows the following actions:
- Edit the configuration.
- Change the environment.
- Delete the configuration override file.