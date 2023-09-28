# Reviews

We use [ReviewService](https://github.com/nventive/ReviewService) for mobile app reviews.

## Configuration

It's configured in [ReviewConfiguration.cs](../src/app/ApplicationTemplate.Presentation/Configuration/ReviewConfiguration.cs).

### Native Review Prompters

The package ReviewService.NativePrompters is installed in the Mobile and Windows projects and provides the implementation of `IReviewPrompter` that actually triggers the store reviews.

### Review Settings Source

We use a custom implementation of `IReviewSettingsSource` called [DataPersisterReviewSettingsSource](../src/app/ApplicationTemplate.Access/LocalStorage/DataPersisterReviewSettingsSource.cs) to persist the review data on disk.

## Reference

- [ReviewService](https://github.com/nventive/ReviewService)
