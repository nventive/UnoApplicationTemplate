# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)

Prefix your items with `(Template)` if the change is about the template and not the resulting application.

## 3.11.X
- Added API Client tests project.
- Removed all references to Uno.SourceGenerationTasks packages and adjusted the dependency injection recipe in ViewModels accordingly.
- Fix Android bottom screen space.
- Fix yaml configuration for tests to always publish test results.
- Append job attempt number for test artifact names in build stage to allow retrying jobs.

## 3.10.X
- Added Dependency Injection validation in the development environment.
- Cleaned up the persistence configuration files (removed unused parameters and updated documentation).
- Updated Contributing documentation.
- Adding a workaround for a bug with the language change on android.
- Cleanup the 'Access' layer's code (renamed repositories into API clients, removed unused namespaces, and sealed some classes).

## 3.9.X
- Removed unnecessary `IsExternalInit.cs` files.
- Renamed multiple components from the data access layer to not all be named `Repository`.
- Moved many components from the presentation layer to the data access layer.
- Disabled simulated API call delays in automated tests.
- Updated Configuration documentation and uno workaround comment.
- Cleanup (fixes warnings on Windows, fixes vulnerabilities and renamed extensions) & Update packages.
- Updated the Segoe MDL2 Assets font to fix an issue with the Flip View icons on Windows.
- Optimized the .NET workloads install process.
- Fixed the iOS application icon size.
- Added VM Disposal in Functional Tests.

## 3.8.X
- Updated from .NET 8 to .NET 9.
- Updated Pipeline Code Coverage Task from V1 to V2.
- Fixed iOS crash by updating package and configuring the interpreter.
- Added support for arm64 and x86 cpus in the mobile project.
- Updated Uno packages from 5.2.121 to 5.6.30.
- Adding workaround for uno safe area issue https://github.com/unoplatform/uno/issues/6218
- Removing MaterialCommandbarHeight property.
- Updated commit validation for the CI/CD.
- Added missing association between JsonContext and Refit.
- Updated versions of CI/CD tasks.
- Fixed downgraded version of Serilog.Sinks.File package.

## 3.7.X
- Replacing Appcenter with Firebase app distribution for Android.
- Refactored the email and connectivity services to be in the access layer.

## 3.6.X
- Added conventional commit validation stage `stage-commit-validation.yml`

## 3.5.X
- Bump Uno packages to 5.2.121 to fix a crash on iOS.
- Ensure NV.Template.Mobile nuget is only deployed from the main branch.
- Updated `System.Text.Json` to resolve security vulnerabilities.
- Remove UWP references in Diagnostics.md
- Updated `Refit` package to 8.0.0 to address security advisory.
- Updated `MallardMessageHandlers` package to 2.0.0.

## 3.4.X
- Added a kill switch feature to the app.
- Bump Uno.WinUI, Uno.WinUI.DevServer, Uno.WinUI.Lottie and Uno.UI.Adapter.Microsoft.Extensions.Logging to 5.0.159 to fix backNavigation/CloseModal crash.
- Fixed an issue with logging configuration not creating the directory before writing the log file in the case logging was disabled by default.
- Fixed Post commands binding issue on Android.

## 3.3.X
- Added a forced update feature to the app.

## 3.2.X
- Added support for mouse back button navigation.
- Set `fetchDepth` to 0 on canary merge CI to avoid `refusing to merge unrelated histories` errors.
- Updated macOS MS-Hosted agents to macOS 13.

## 3.1.X
- Updated to .NET 8.
- Added MAUI essentials on Windows.
- Fixed MAUI essentials warning.
- Fixed WinUI crashes in diagnostics screens.

## 3.0.X
- Updated to Uno 5.
- Fix canary builds by updating canary merge yml

## 2.2.X
- Added hooks for default analytics (page views and command invocations).
- Renamed the `AnalyticsDataLoaderStrategy` to `MonitoringDataLoaderStrategy`. (The same renaming was applied to related methods and classes).
- Remove all triggers on the API Integration tests CI.
- Ensures API Integration tests CI runs every day of the week.

## 2.1.X
- Install `GooseAnalyzers` to enable the `SA1600` rule with its scope limited to interfaces and improve xml documentation.
- Replace local `DispatcherQueue` extension methods with the ones from the WinUI and Uno.WinUI Community Toolkit.
- Add `Microsoft.VisualStudio.Threading.Analyzers` to check for async void usages and fix async void usages.
- Enable `TreatWarningsAsErrors` for the Access, Business, and Presentation projects.
- Update analyzers packages and severity of rules.
- Fix crash from ARM base mac on net7.0-iOS. Add `ForceSimulatorX64ArchitectureInIDE` property to mobile head.
- Consolidate Windows and macOS build agents to Microsoft-hosted agents.

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
