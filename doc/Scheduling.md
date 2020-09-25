# Scheduling

## UI scheduling

If you need access to the UIThread, you can simply use the `IDispatcherScheduler` that is registered in the IoC.

If you're in a view model, you can simply use `RunOnDispatcher`.

## Background scheduling

If you need access to a background thread, you can simply use the `IBackgroundScheduler` that is registered in the IoC.