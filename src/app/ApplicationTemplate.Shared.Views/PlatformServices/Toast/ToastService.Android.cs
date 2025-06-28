// src/app/ApplicationTemplate.Shared.Views/PlatformServices/Toast/ToastService.Android.cs
#if __ANDROID__
using Android.Widget;
using ApplicationTemplate.DataAccess.PlatformServices;

namespace ApplicationTemplate.DataAccess.PlatformServices;

public partial class ToastService : IToastService
{
    public void ShowNotification(string message, ToastDuration duration = ToastDuration.Short)
    {
        Toast.MakeText(Android.App.Application.Context, message, duration == ToastDuration.Short ? ToastLength.Short : ToastLength.Long).Show();
    }
}
#endif
