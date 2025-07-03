// src/app/ApplicationTemplate.Access/PlatformServices/Version/VersionProvider.Android.cs
#if __ANDROID__
using System;
using Android.Content;
using Android.Content.PM;


namespace ApplicationTemplate.DataAccess.PlatformServices;


public class VersionProvider : IVersionProvider
{
    private readonly Lazy<PackageInfo> _packageInfo;


    public VersionProvider()
    {
        _packageInfo = new Lazy<PackageInfo>(() =>
        {
            var context = Platform.CurrentActivity?.ApplicationContext ?? Android.App.Application.Context;
            return context.PackageManager.GetPackageInfo(context.PackageName, 0);
        });
    }


    public string BuildString => _packageInfo.Value.LongVersionCode.ToString();


    public Version Version
    {
        get
        {
            var versionName = _packageInfo.Value.VersionName;
            if (System.Version.TryParse(versionName, out var version))
            {
                return version;
            }
            return new Version(1, 0, 0);
        }
    }


    public string VersionString => Version.ToString(3);
}
#endif
