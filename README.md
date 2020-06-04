# Uno Application Template

This is a mobile app project template using latest practices.

We use `dotnet` project templates to easily create new projects. It simplifies the project renaming and supports conditional inclusions.

## Getting Started

1. Install the template using the following command at the root of the project.

    `dotnet new -i ./`

1. If you want to uninstall the template, run the following command.

    `dotnet new -u`

    This will list you the list of installed templates, look for this template and copy the command with the absolute path like this. (Note the quotes added, otherwise it doesn't work)

    `dotnet new -u "C:\P\ApplicationTemplate"`

1. To run the template and create a new project, run the following command.

    `dotnet new nv-mobile -n MyProjectName`

    The following options are available when running the command.

    - To get help: `dotnet new nv-mobile -h`
    - To add Firebase Analytics: `dotnet new nv-mobile -n MyProjectName --include-firebase-analytics` (or -fa)

[Read this for more information on custom templates](https://docs.microsoft.com/en-us/dotnet/core/tools/custom-templates).

## Architecture

Please consult the [Architecture document](Doc/Architecture.md) for more information about the project.

## Changelog

Please consult the [CHANGELOG](CHANGELOG.md) for more information about the version history.

## License

This project is licensed under the Apache 2.0 license. See the [LICENSE](LICENSE) for details.

## Contributing

Please read [CONTRIBUTING](CONTRIBUTING.md) for details on the process for contributing to this project.

Be mindful of our [Code of Conduct](CODE_OF_CONDUCT.md).
