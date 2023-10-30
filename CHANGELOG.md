# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)

Prefix your items with `(Template)` if the change is about the template and not the resulting application.

## 2.1.X
- Replace local `DispatcherQueue` extension methods with the ones from the WinUI and Uno.WinUI Community Toolkit.
- Add `Microsoft.VisualStudio.Threading.Analyzers` to check for async void usages and fix async void usages.
- Enable `TreatWarningsAsErrors` for the Access, Business, and Presentation projects.
- Update analyzers packages and severity of rules.

## 2.0.X
- Renamed the classes providing data to use the `Repository` suffix instead of `Endpoint` or `Service`.
- Renamed the Client library to Access.
  - This properly orders the layers in the solution explorer (alphabetically).
- Renamed the Client namespace to DataAccess.

## 1.1.X
- Update Uno Material packages to latest version.
- Fix colors not changing when changing the theme.
- Replaced Moq by NSubstitute.

## 1.0.X
- Removed the UI tests project.
- Split the automated tests into 2 projects: 
  - **ApplicationTemplate.Tests.Unit** for unit tests.
  - **ApplicationTemplate.Tests.Functional** for functional tests.
- Added `azure-pipelines-api-integration-tests.yml` as an API Integration Tests Pipeline.
- Removed the installation of the mobile dotnet workloads for pipelines test steps.

## 0.76.X
* Removed deprecated info.plist keys.
* Removed unused and deprecated package.
* Fixed WinUI app always being in French.
* Improved architecture documentation by adding a **Technical Overview** section listing all major recipes.
* Renamed the `build` solution folder to `root` to better represent its content and added new `build` solution folder containing the content of the `build` folder.
* Moved `gitversion-config.yml` to the `build` folder.
* Updated pipeline documentation.
* (Template) Improved PR templates by mentionning documentation files to update.
* Hide the xaml file links in the Windows head.
* (Template) Fixed usage of guids to prevent having conflicts with generating multiple projects with the template.
* Renamed `script` folder to `tools`.
* Removed the following files because they're associated with the template and not the generated app.
  - `.mergify.yml`
  - `LICENSE`
  - `CHANGELOG.md`
  - `CODE_OF_CONDUCT.md`
  - `CONTRIBUTING.md`
* (Template) Introduce the APP_README.md template to generate a better README for the generated projects.
* Update the solution structure image.
* Added window title to Windows.
* Removed AsyncWebView.

## 0.75.X - 2023-08-22
Initial release on nuget.org.

### Changed
* (Template) Updated installation documentation.
* (Template) Update GitHub PR template to include checks around versioning.

## Before Initial Release

### Added
* Support for Android 13 (May, 2023)
* Support for C# 10, VS 2022 required (August 2022)
* Support for Android 12 (Avril, 2022)
* Support for Android 11 (March, 2021)

### Changed
* Update template to support .NET 7
* Improved the build pipeline cleanup process
* Concurrent access token refreshing

### Deprecated

### Removed

### Fixed

### Security
