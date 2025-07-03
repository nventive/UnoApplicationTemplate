// src/app/ApplicationTemplate.Access/PlatformServices/Version/VersionProvider.Windows.cs
#if __WINDOWS__
using System;
using System.Reflection;
using Windows.ApplicationModel;


namespace ApplicationTemplate.DataAccess.PlatformServices;


public class VersionProvider : IVersionProvider
{
    private readonly Lazy<PackageVersion> _packageVersion;


    public VersionProvider()
    {
        _packageVersion = new Lazy<PackageVersion>(() => Package.Current.Id.Version);
    }


    public string BuildString => _packageVersion.Value.Revision.ToString();


    public Version Version => new Version(
        _packageVersion.Value.Major,
        _packageVersion.Value.Minor,
        _packageVersion.Value.Build);


    public string VersionString => Version.ToString(3);
}
#endif
