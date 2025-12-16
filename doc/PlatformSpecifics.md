# Platform specifics

## Android

### AndroidManifest
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
3. Execute the following command: `dotnet build -f net10.0-android36.0 -t:BuildAndStartAotProfiling`
    - The dotnet version targeted must match the one specified in the mobile csproj.
    - If you have a custom Android SDK path, you can specify it with the `AndroidSdkPath` property.
      > 💡 `-p:AndroidSdkDirectory=path/to/android/sdk`
4. Let your application run until it’s loaded.
5. Execute the following command: `dotnet build -f net10.0-android36.0 -t:FinishAotProfiling`.
    - The dotnet version targeted must match the one specified in the mobile csproj.
    - If you have a custom Android SDK path, you can specify it with the `AndroidSdkPath` property.
      > 💡 `-p:AndroidSdkDirectory=path/to/android/sdk`
6. Use this configuration in your `.csproj`.

```xml
<EnableLLVM>True</EnableLLVM>
<AndroidEnableProfiledAot>True</AndroidEnableProfiledAot>
<AndroidUseDefaultAotProfile>False</AndroidUseDefaultAotProfile>
<PackageReference Include="Mono.AotProfiler.Android" Version="10.0.0-preview1" />
<AndroidAotProfile Include="$(MSBuildThisFileDirectory)custom.aprof" />
```

- [For more information, check this article.](https://devblogs.microsoft.com/dotnet/performance-improvements-in-dotnet-maui)
- [For more documentation, check this GitHub.](https://github.com/jonathanpeppers/Mono.Profiler.Android)


## iOS

### Interpreter

If the iOS application is crashing instantly on launch, it may be caused by the AOT compilation (ahead of time) for iOS.
Some NuGet packages don’t always support the necessary AOT conditions and instead are JIT compiled (just in time).

1. Connect your iPhone to your Mac via cable.
2. Open the 'Console' application on your Mac.
   > 💡 [Console User Guide for Mac – Apple Support (SG)](https://support.apple.com/en-sg/guide/console/welcome/mac)
3. Choose your phone and click on start stream.
4. Open the application that has the problem.
5. Search for the error in the Mac 'Console' application, it should provide you with the assembly that is causing the issue.
   > 💡 Search for 'Failed to load AOT module'.
6. Then include it in the list of assemblies that should be interpreted.
   ```xml
   <ItemGroup>
	   <MtouchInterpreter>-all,InterpretedAssembly</MtouchInterpreter>
   </ItemGroup>
   ```
7. Try again, sometime you may need to add more assemblies to the list.


## Windows
