# Your Project Name

TODO: Give a **brief description** of the app.
Explain the **high level goals** and the **main** features.

TODO: Make sure you adjust or augment the information that is provided by default in this document.
e.g. If you decide to change the solution structure, you should mention it in the [Solution Structure section](#solution-structure).

## Project Template

This repository was generated using the **nventive Mobile Template**.
- Version: [{{app-template-version}}](https://www.nuget.org/packages/NV.Templates.Mobile/{{app-template-version}})
- Commit: [{{app-template-commit-short-sha}}](https://github.com/nventive/UnoApplicationTemplate/tree/{{app-template-commit-full-sha}})
- Date: {{app-template-commit-date}}

[**View the template changes since the generation of this project**.](https://github.com/nventive/UnoApplicationTemplate/compare/{{app-template-commit-full-sha}}..main)

## Environment and Prerequisites

### Local Development Requirements
All development is expected to be done from Visual Studio in a Windows environment.

- .Net 7
- Visual Studio 2022 (17.4 and above)
  - We recommend validating your components using this [Uno guide](https://platform.uno/docs/articles/get-started-vs-2022.html).
- For mobile development, MAUI workloads are required.
  - You can install them using [`uno-check`](https://platform.uno/docs/articles/external/uno.check/doc/using-uno-check.html).
- For local iOS compilation and debugging, you need access to Mac with Xcode 14.2 (more recent versions may work too).

### Pipelines Requirements
The pipelines (for continuous integration, testing, and delivery) of this project are made for [Azure Pipelines](https://learn.microsoft.com/en-us/azure/devops/pipelines/get-started/what-is-azure-pipelines?view=azure-devops).

- You need an Azure DevOps organization project with Pipelines enabled.
  - You can follow [this guide](https://learn.microsoft.com/en-us/azure/devops/pipelines/get-started/pipelines-sign-up?view=azure-devops) for more details.
- For compilation, access to the the following [Microsoft-hosted agents](https://learn.microsoft.com/en-us/azure/devops/pipelines/agents/hosted?view=azure-devops&tabs=yaml) is required.
  - `windows-2022`
  - `macOS-12`
- For deployment, access to Mac agents with the following capabilities is required.
  - `fastlane 2.212.1`

## Repository Content

| Folder or File | Description |
|-:|-|
`.azuredevops/` | Used to store the pull request template used by Azure DevOps.
`build/` | Regroups all yaml files used that compose the pipelines defined in `.azure-pipelines.yml`.
`.azure-pipelines.yml` | Defines the main CI/CD pipeline of the project.
`.azure-pipelines-canary.yml` | Defines the canary pipeline of the project.<br/>Canary pipelines allow to detect regressions by periodically creating versions of the app in which all the dependencies are updated to their latest version.
`doc/` | Regroups all the documentation of the project, with the only exception of `README.md`, which is located at the root of the repository.<br/><br/>**There are many topics covered. Make sure you check them out.**
`README.md` | The entry point of this project's documentation.
`src/` | Contains all the code of the application, including tests, with the exception of the configuration files located at the root of the repository.
`src/{YourProjectName}.sln` | The solution file to use with Visual Studio to develop locally.
`.editorconfig`<br/>& `stylecop.json` | Configure the code formatting rules and analyzers of Visual Studio.
`.gitignore` | Contains the git ignore rules.
`Directory.Build.props` | Regroups build configuration that apply to all `.csproj`.
`gitversion-config.yml` | Contains the configuration for the GitVersion tool that is used to compute the version number in the pipelines.
`nuget.config` | Contains the configuration for NuGet packages.
`tools/` | Offers a place to put custom tools and scripts. It also contains some information about the version of the template that was used to generate the project.

## Solution Structure

See [Solution Structure in Architecture.md](doc/Architecture.md#solution-structure).

## Pipelines

### Required Knowledge
If you're unfamiliar with Azure Pipeline, you should at least read about the following topics.
- [Key Concepts](https://learn.microsoft.com/en-us/azure/devops/pipelines/get-started/key-pipelines-concepts?view=azure-devops)
- [Set a secret variable in a variable group](https://learn.microsoft.com/en-us/azure/devops/pipelines/process/set-secret-variables?view=azure-devops&tabs=yaml%2Cbash#set-a-secret-variable-in-a-variable-group)
- [Add a secure file](https://learn.microsoft.com/en-us/azure/devops/pipelines/library/secure-files?view=azure-devops#add-a-secure-file)

### Pipeline Code
This project uses CI/CD pipelines that are implemented as yaml code.
They are declared in the following files.
- `.azure-pipelines.yml`
- `.azure-pipelines-canary.yml`

These pipelines are divided in parameterized stages that are defined accross several files, all located under [`build/`](build/).
The more complex stages are also divided into several steps files, again all located under the [`build/`](build/) folder.

At high level, the CI/CD pipelines do the following:
- **Build** the app in **staging**.
  - **Deploy** the staging app (to AppCenter and/or TestFlight and GooglePlay).
- **Build** the app in **production**.
  - **Deploy** the production app (to TestFlight and GooglePlay).

They also run automated tests during the build steps.

### Variable Groups and Secrets
These pipelines rely on a few variable groups and secrets in order to fully work. These are documented in [`variables.yml`](build/variables.yml).

### Pipelines in Azure DevOps
TODO: Fill the following table with your own pipelines.

| Link | Code Entry Point | Goal | Triggers |
|-|-|-|-|
| [Name of Main Pipeline](link-to-pipeline)| [`.azure-pipelines.yml`](.azure-pipelines.yml)| Build validation during pull request.| Pull requests.
| [Name of Main Pipeline](link-to-pipeline)| [`.azure-pipelines.yml`](.azure-pipelines.yml)| Build and deploy the application to AppCenter, TestFlight, and GooglePlay. | Changes on the `main` branch.<br/>Manual trigger.
| [Name of Canary Merge Pipeline](link-to-pipeline)| [`build/canary-merge.yml`](.azure-pipelines.yml)| Creation of canary branches (`canary/build/*`). | Daily cron job.
| [Name of Canary Pipeline](link-to-pipeline)| [`.azure-pipelines-canary.yml`](.azure-pipelines.yml)| Build and deploy canary versions of the app to AppCenter and TestFlight. | Upon creation of branches with the `canary/build/*` pattern.


## Additional Information

TODO: Add any other relevant information. e.g. You could link to your _Definition of Done_, more documentation, UI mockups, etc.
