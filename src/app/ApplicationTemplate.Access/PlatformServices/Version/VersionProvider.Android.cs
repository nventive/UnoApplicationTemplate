// src/app/ApplicationTemplate.Access/PlatformServices/Version/VersionProvider.Android.cs
using Android.Content;

namespace ApplicationTemplate.DataAccess.PlatformServices
{
	public class VersionProvider : IVersionProvider
	{
		private readonly Context _context;

		public VersionProvider(Context context) => _context = context;

		public string BuildString => _context.PackageManager.GetPackageInfo(_context.PackageName, 0).VersionCode.ToString();
		public Version Version => new Version(
			int.Parse(_context.PackageManager.GetPackageInfo(_context.PackageName, 0).VersionName.Split('.')[0]),
			int.Parse(_context.PackageManager.GetPackageInfo(_context.PackageName, 0).VersionName.Split('.')[1]),
			int.Parse(_context.PackageManager.GetPackageInfo(_context.PackageName, 0).VersionName.Split('.')[2]),
			_context.PackageManager.GetPackageInfo(_context.PackageName, 0).VersionCode
		);
		public string VersionString => _context.PackageManager.GetPackageInfo(_context.PackageName, 0).VersionName;
	}
}
