## Platform specifics

### Android

#### AndroidManifest
- Information usually found in AndroidManifest is split into three files.
    - `AndroidManifest.xml` for various options, intent filters providers etc.
    - `AssemblyInfo.cs` for the permissions.
    - `Main.cs` for the `<Application>` properties. 

#### ApplicationName
- `Label` is set in Main.cs to a resource. For Android this ApplicationName resource is located in `Resources/values/Strings.xml`. Resources.resw in the Shared project is not used for the label on Android.

### iOS



### UWP



### WASM


