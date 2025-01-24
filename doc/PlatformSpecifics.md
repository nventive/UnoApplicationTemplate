# Platform specifics

## Android

#### AndroidManifest
- Information usually found in AndroidManifest is split into three files.
    - `AndroidManifest.xml` for various options, intent filters providers etc.
    - `AssemblyInfo.cs` for the permissions.
    - `Main.cs` for the `<Application>` properties. 

### ApplicationName
- `Label` is set in Main.cs to a resource. For Android this ApplicationName resource is located in `Resources/values/Strings.xml`. Resources.resw in the Shared project is not used for the label on Android.

### Profiled AOT

In order to get better startup performance on Android, this application is bootstrapped using profiled AOT.

This is a special type of compilation that uses a generated file (`custom.aprof`) to optimize the AOT compilation.

To generate this file, following the following steps:

1. Open a command prompt or terminal against your Android project’s directory that contains the .csproj.
2. Ensure only one Android device is attached.
3. Execute the following command: `dotnet build -f net9.0-android35.0 -t:BuildAndStartAotProfiling`
    - The dotnet version targeted must match the one specified in the mobile csproj.
    - If you have a custom Android SDK path, you can specify it with the `AndroidSdkPath` property.
      > 💡 `-p:AndroidSdkDirectory=path/to/android/sdk`
4. Let your application run until it’s loaded.
5. Execute the following command: `dotnet build -f net9.0-android35.0 -t:FinishAotProfiling`.
    - The dotnet version targeted must match the one specified in the mobile csproj.
    - If you have a custom Android SDK path, you can specify it with the `AndroidSdkPath` property.
      > 💡 `-p:AndroidSdkDirectory=path/to/android/sdk`
6. Use this configuration in your `.csproj`.

```xml
<EnableLLVM>True</EnableLLVM>
<AndroidEnableProfiledAot>True</AndroidEnableProfiledAot>
<AndroidUseDefaultAotProfile>False</AndroidUseDefaultAotProfile>
<PackageReference Include="Mono.AotProfiler.Android" Version="7.0.0" />
<AndroidAotProfile Include="$(MSBuildThisFileDirectory)custom.aprof" />
```

- [For more information, check this article.](https://devblogs.microsoft.com/dotnet/performance-improvements-in-dotnet-maui)
- [For more documentation, check this GitHub.](https://github.com/jonathanpeppers/Mono.Profiler.Android)

### iOS

### Windows
