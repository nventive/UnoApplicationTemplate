# Forced Update

The forced update feature is for when you want to force the user to update the app.
You could use this, for example, when the backend changes and you do not want the users to still use the old API.

To force an update, we suscribe to the `UpdateRequired` `event` from the `UpdateRequiredservice` in the [CoreStartup](../src/app/ApplicationTemplate.Presentation/CoreStartup.cs#L203).

This will direct the user to a page from which they cannot navigate back. The page will contain a button that leads them to the appropriate page for updating the app, with the links defined in `AppSettings`.
