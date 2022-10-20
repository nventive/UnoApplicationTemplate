using System;
using Android.App;
using Android.Runtime;
using Com.Nostra13.Universalimageloader.Core;
using Microsoft.UI.Xaml.Media;

namespace ApplicationTemplate;

[Application(
	Label = "@string/ApplicationName",
	LargeHeap = true,
	HardwareAccelerated = true,
	Theme = "@style/AppTheme",
	AllowBackup = false,
	ResizeableActivity = false
)]
public class Application : Microsoft.UI.Xaml.NativeApplication
{
	public Application(IntPtr javaReference, JniHandleOwnership transfer)
		: base(() => new App(), javaReference, transfer)
	{
		ConfigureUniversalImageLoader();
	}

	private void ConfigureUniversalImageLoader()
	{
		var config = new ImageLoaderConfiguration
			.Builder(Context)
			.Build();

		ImageLoader.Instance.Init(config);

		ImageSource.DefaultImageLoader = ImageLoader.Instance.LoadImageAsync;
	}
}
