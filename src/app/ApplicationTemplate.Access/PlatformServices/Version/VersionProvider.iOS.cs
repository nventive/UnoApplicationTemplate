// src/app/ApplicationTemplate.Access/PlatformServices/Version/VersionProvider.iOS.cs
#if __IOS__
using System;
using Foundation;


namespace ApplicationTemplate.DataAccess.PlatformServices;


public class VersionProvider : IVersionProvider
{
    private readonly Lazy<NSDictionary> _infoDictionary;


    public VersionProvider()
    {
        _infoDictionary = new Lazy<NSDictionary>(() => NSBundle.MainBundle.InfoDictionary);
    }


    public string BuildString
    {
        get
        {
            var buildNumber = _infoDictionary.Value["CFBundleVersion"]?.ToString();
            return buildNumber ?? "-1";
        }
    }


    public Version Version
    {
        get
        {
            var versionString = _infoDictionary.Value["CFBundleShortVersionString"]?.ToString();
            if (System.Version.TryParse(versionString, out var version))
            {
                return version;
            }
            return new Version(1, 0, 0);
        }
    }


    public string VersionString => Version.ToString(3);
}
#endif
