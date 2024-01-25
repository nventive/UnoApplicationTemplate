﻿using System;

namespace ApplicationTemplate;

public interface ISavedSettingsService
{
	void LoginSaved();

	void SetSavedString(string key, string data);

	string ReadSavedString(string key);

	void DeleteSaved(string key);
}
