# Uno Platform Application Template

This is a mobile app project template using [Uno Platform](https://github.com/unoplatform/uno) and the latest .NET practices.

- It uses the MVVM pattern.
- Code is organized by [application layer](doc/Architecture.md#Solution-Structure).
- It comes with [dependency injection](doc/DependencyInjection.md).
- There are built-in [logs](doc/Logging.md) and [diagnostics tools](doc/Diagnostics.md).
- There is scaffolding code showing sample features.
  When you run as-is, you get a _Dad Jokes_ application.

## Requirements

Visual Studio 2022 with .Net 7 are required.

This template largely relies on Uno Platform, if you want to make sure you got everything installed correctly on your machine, we encourage you to use `uno-check`, the documentation is available [here](https://platform.uno/docs/articles/uno-check.html)

> 💡 It's also possible to use this template for a pure WinUI application, without any mobile aspect.
> All you would have to do is remove the `.Mobile` csproj from the generated solution.


## Getting Started

We use `dotnet` project templates to easily create new projects. It simplifies the **project renaming** and supports **conditional inclusions**.

### Generate a new project

1. Install the template using this command.
   
   `donet new install NV.Templates.Mobile`

1. To run the template and create a new project, run the following command in the folder that will contain the new project.
    
    `dotnet new nv-mobile -n MyProjectName`
    
    > ⚠ The use of periods (`.`) in the project name is not supported and may result in compilation issues later on.

   > 💡 If all your projects are regrouped in a folder like `C:\Repos`, you want to be in that folder.
   > The command would generate all the project files under `C:\Repos\MyProjectName`.

   The following options are available when running the command.

   - To get help: `dotnet new nv-mobile -h`

### Next Steps

1. Open the `README.md` and complete the documentation TODOs.
1. Open the solution file from the generated folder using Visual Studio. 

   It's located at `MyProjectName/src/MyProjectName.sln`.

1. In Visual Studio, go to the **VIEW** menu and open the **Task List** to get hints on next steps.
   
   This template comes with several pointers on what you're most likely to change next.
   
   ![](doc/images/VisualStudioTaskListForNextSteps.PNG)

## Documentation

This repository provides documentation on different topics under the [doc](doc/) folder.

- [Architecture](doc/Architecture.md)
- [Startup](doc/Startup.md)
- [Dependency Injection](doc/DependencyInjection.md)
- [Configuration](doc/Configuration.md)
- [HTTP](doc/HTTP.md)
- [Logging](doc/Logging.md)
- [Diagnostics](doc/Diagnostics.md)
- [Platform specifics](doc/PlatformSpecifics.md)
- [Serialization](doc/Serialization.md)
- [Testing](doc/Testing.md)
- [Environments](doc/Environments.md)
- [Localization](doc/Localization.md)
- [Validation](doc/Validation.md)
- [Error handling](doc/ErrorHandling.md)
- [Scheduling](doc/Scheduling.md)
- [Reviews (app star ratings)](doc/Reviews.md)

## Debugging or Testing the Template
Here's how to install the template directly from the code, in the case that you want to modify it and would like to test your changes.

### Installing the template

1. Uninstall the template from nuget.org (if applicable).
   - `dotnet new uninstall NV.Templates.Mobile`

1. Clone this repository on your machine.
1. Open a command prompt at the root of the cloned repository.
1. Run the following command.

    - `dotnet new install ./`

[Read this for more information on custom templates](https://docs.microsoft.com/en-us/dotnet/core/tools/custom-templates).

### Uninstalling the template
1. Open a command prompt at the root of the cloned repository. 
1. Run the following command.

    - `dotnet new uninstall ./`

## Changelog

Please consult the [CHANGELOG](CHANGELOG.md) for more information about the version history.

## License

This project is licensed under the Apache 2.0 license. See the [LICENSE](LICENSE) for details.

## Contributing

Please read [CONTRIBUTING](CONTRIBUTING.md) for details on the process for contributing to this project.

Be mindful of our [Code of Conduct](CODE_OF_CONDUCT.md).
