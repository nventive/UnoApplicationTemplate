# Uno Platform Application Template

This is a mobile app project template using [Uno Platform](https://github.com/unoplatform/uno) and the latest .NET practices.

- It uses the MVVM pattern.
- Code is organized by [application layer](doc/Architecture.md#Solution-Structure).
- It comes with [dependency injection](doc/DependencyInjection.md).
- There are built-in [logs](doc/Logging.md) and [diagnostics tools](doc/Diagnostics.md).
- There is scaffolding code showing sample features.
  When you run as-is, you get a _Dad Jokes_ application.

## Getting Started

We use `dotnet` project templates to easily create new projects. It simplifies the **project renaming** and supports **conditional inclusions**.

### Installing the template

1. In order to install the template, clone this repository on your machine, open a command prompt at its root and run the following command.

    - dotnet 7 and up: `dotnet new install ./`
    - older dotnet version: `dotnet new -i ./`

    This will display a list of installed templates, look for this template and copy the command with the absolute path like this.

    - dotnet 7 and up: `dotnet new list`
    - older dotnet version: `dotnet new --list`

### Running the template to generate a new project

1. To run the template and create a new project, run the following command in the root folder that will contain the new project.

    `dotnet new nv-mobile -n MyProjectName`

    The following options are available when running the command.

    - To get help: `dotnet new nv-mobile -h`

	:warning: The use of periods (.) in the project name is not supported and may result in building issues. 

[Read this for more information on custom templates](https://docs.microsoft.com/en-us/dotnet/core/tools/custom-templates).

### Uninstalling the template
1. If you want to uninstall the template, open a command prompt at its root and run the following command.

    - dotnet 7 and up: `dotnet new uninstall ./`
    - older dotnet version: `dotnet new -u ./`

## Requirements

This template largely relies on Uno Platform, if you want to make sure you got everything installed correctly on your machine, we encourage you to use `uno-check`, the documentation is available [here](https://platform.uno/docs/articles/uno-check.html)

In addition to that, **Visual Studio 2022 is required** since C# 10 is used.

## Documentation

This repository provides documentation on different topics under the [doc](doc/) folder.

- [Architecture](doc/Architecture.md)
- [Dependency Injection](doc/DependencyInjection.md)
- [Configuration](doc/Configuration.md)
- [HTTP](doc/HTTP.md)
- [Logging](doc/Logging.md)
- [Diagnostics](doc/Diagnostics.md)
- [Platform specifics](doc/PlatformSpecifics.md)
- [Serialization](doc/Serialization.md)
- [Startup](doc/Startup.md)
- [Testing](doc/Testing.md)
- [Environments](doc/Environments.md)
- [Localization](doc/Localization.md)
- [Validation](doc/Validation.md)
- [Error handling](doc/ErrorHandling.md)
- [Scheduling](doc/Scheduling.md)

## Changelog

Please consult the [CHANGELOG](CHANGELOG.md) for more information about the version history.

## License

This project is licensed under the Apache 2.0 license. See the [LICENSE](LICENSE) for details.

## Contributing

Please read [CONTRIBUTING](CONTRIBUTING.md) for details on the process for contributing to this project.

Be mindful of our [Code of Conduct](CODE_OF_CONDUCT.md).
