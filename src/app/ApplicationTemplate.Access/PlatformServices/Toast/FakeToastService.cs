namespace CPS.DataAccess.PlatformServices;

public sealed class FakeToastService : IToastService
{
	public void ShowNotification(string message, ToastDuration duration = ToastDuration.Short)
	{
		return;
	}
}
