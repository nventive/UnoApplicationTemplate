# Diagnostics Tools

This template comes with multiple built-in diagnostics tools.

## Diagnostics Overlay
When you start the application in debug, you'll notice a box on the side of the screen.
This overlay shows various counters and buttons.
The overlay is accessible from anywhere in your app.
This is useful when you want to see something happening live.

**Feel free to add more counters and buttons to ease your work!**

The default counters track the amount of active and uncollected ViewModel, DynamicProperties, and DynamicCommands.
The default buttons include commands such as the following.
- **Collect** runs the garbage collector.
- **Theme** toggles between the light and dark theme for the whole application.
- **<** opens the expanded view of the overlay.

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
- Open the settings folder where the settings files can be found (UWP only).

## Http Debugger

On the diagnostics overlay, tap on the **Http** button to show the `HttpDebuggerView`.
This view shows all the http calls and their status.
You can click on items to see more details, such as headers, content, elapsed time, etc.
Check the following for more details.
- [`IHttpDebuggerService`](..\src\app\ApplicationTemplate.Client\Framework\HttpDebugger\IHttpDebuggerService.cs)
- [`HttpDebuggerHandler`](..\src\app\ApplicationTemplate.Client\Framework\HttpDebugger\HttpDebuggerHandler.cs)
- [`HttpDebuggerViewModel`](..\src\app\ApplicationTemplate.Presentation\ViewModels\Diagnostics\HttpDebugger\HttpDebuggerViewModel.cs)
- [`HttpDebuggerView`](..\src\app\ApplicationTemplate.UWP\Views\Content\Diagnostics\HttpDebuggerView.xaml)