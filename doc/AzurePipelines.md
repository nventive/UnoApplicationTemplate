# Azure Pipelines

## Pipeline Code
This project uses CI/CD pipelines that are implemented as yaml code.
They are declared in the following files.
- `.azure-pipelines.yml`
- `.azure-pipelines-api-integration-tests.yml`
- `build/canary-merge.yml`
- `.azure-pipelines-canary.yml`

These pipelines are divided in parameterized stages that are defined accross several files, all located under [`build/`](build/).
The more complex stages are also divided into several steps files, again all located under the build folder.

## Variable Groups and Secrets
These pipelines rely on a few variable groups and secrets in order to fully work. These are documented in [`variables.yml`](build/variables.yml).

## Main Pipeline [.azure-pipelines.yml](../.azure-pipelines.yml)

At high level, the CI/CD pipelines do the following:
- **Build** the app in **staging**.
  - **Deploy** the staging app (to AppCenter and/or TestFlight and GooglePlay).
- **Build** the app in **production**.
  - **Deploy** the production app (to TestFlight and GooglePlay).

It also runs automated tests during the build steps.

### Pull request runs
Due to the length of mobile builds, pipelines are configured to behave a little differently when building in a context of **pull request (PR) build validation**. To reduce the build time, some runtime performance optimizations are disabled for PR builds. This requires a specific variable called `IsLightBuild` to be set, hence why it is appearing in the pipeline. 

Also, all release stages are disabled in the context of PR build validation because, with the optimizations differences, the resulting application would not represent the real thing.

### Release runs
Pipeline runs **triggered on the main branch** don't qualify for `IsLightBuild` and build the application with the goal of releasing it.
All stages are therefore enabled.

## Stages and Steps
### Build Stage ([stage-build.yml](../build/stage-build.yml))
This file represents the stage that builds the applications on all platforms for any specified environment.
It has many parameters, **the most important being the following**:
- Application environment
- Secured files (including provisioning profiles, certificates, key stores)
- Variables groups (containing applications ids, certificates passwords, etc.)

Note that variable groups and secured files are described in details in [`variables.yml`](../build/variables.yml).

This stage captures all the parameters required to build the application for all platforms and dispatches to work on parallel jobs. Each job is responsible for building the app on **one** platform.
- The iOS job runs on a **Mac** agent (because iOS apps can only be built on a Mac).
- All other jobs run on a **Windows** agent.

The steps for each job are encapsulated in **steps files**.
| Steps file | Goal |
|-|-|
`steps-build-android.yml`| Builds the Android app.
`steps-build-ios.yml`| Builds the iOS app.
`steps-build-windows.yml`| Builds the WinUI app.
`steps-build-tests.yml`| Builds and runs the tests.
`steps-build-release-notes.yml`| Generates release notes.

### Build Steps (steps-build.{...}.yml)
This is where the exact build steps are defined. These vary depending on the platform, but can be summarized as follows:
1. Install the dotnet workloads.
1. Install and run [GitVersion](https://gitversion.net/) to calculate the semantic version based on the Git history.
1. Install the proper signing certificates (depending on the platform).
1. Run the build with `dotnet build` or `dotnet publish` (depending on `IsLightBuild`).
1. Run the tests and publish both the test results and the code coverage results.
1. Push the built artifacts (.ipa, .aab, release notes, etc.).
1. Cleanup.

The release stages are even more straigtforward than the build ones. One thing to note is that, for the same reason as it is done at the end of the build steps, a clean-up step is included in every stage.

### AppCenter Release Stage ([stage-release-appcenter.yml](../build/stage-release-appcenter.yml))
This stage is in charge of pushing the application to AppCenter. It's divided into 2 jobs, one for each platform.

### Apple AppStore Release Stage ([stage-release-appstore.yml](../build/stage-release-appstore.yml))
This stage is in charge of pushing the iOS version to the Apple AppStore. Given that the build stage signs the application, this is as simple as using the proper task and pushing the **IPA** file. This should only be run for configurations that properly sign the application.

### GooglePlay Console Release Stage ([stage-release-googleplay.yml](../build/stage-release-googleplay.yml))
Similar to the App Store stage, this stage pushes the **AAB** produced by the build to the Google Play Store. This is also meant for a properly signed AAB.

## API Integration Tests Pipeline [.azure-pipelines-api-integration-tests.yml](../.azure-pipelines-api-integration-tests.yml)
This pipeline is in charge of running all tests, including the API integration tests.
It runs all the tests and merges their coverage results into one file to have a better idea of the overall coverage.

The API integration tests are the ones defined in the Functional Tests project.
Their configuration is simply changed so that they use real endpoint implementations instead of mocked implementations.
This is done by setting the environment variable `USE_REAL_APIS` to `true`.

This pipeline should be setup as a **scheduled pipeline** that runs every night and **should NOT be part of build validation**. PRs should not be blocked when APIs are down.

## Canary Pipelines
Canary pipelines allow to detect regressions by periodically creating versions of the app in which all the dependencies are updated to their latest version.

This works with 2 pipelines.
- Canary Merge Pipeline (`build/canary-merge.yml`)
- Canary Deployment Pipeline (`.azure-pipelines-canary.yml`)

> The term "canary" comes from [_canary in a coal mine_](https://en.wikipedia.org/wiki/Sentinel_species#Historical_examples) and refers to detecting problems before they become too important.

### Canary Merge
This pipelines creates a branch on which it commits a version of the latest code with all nuget dependencies updated to their latest version. It does so using a custom task called `nventiveCanaryUpdater`, which can be found [here](https://github.com/nventive/nventive-Build-Tools/blob/master/overview.md#canary-updater).

### Canary Deployment
This pipelines triggers automatically when a new branch is created and pushed by the previous pipeline. It takes the new code, builds it, and deploys so that it can be manually tested.

This pipeline uses the same build and release stages as the main CI/CD pipeline of the app.