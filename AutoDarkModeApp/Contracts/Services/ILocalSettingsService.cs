namespace AutoDarkModeApp.Contracts.Services;

public interface ILocalSettingsService
{
    Task<T?> ReadSettingAsync<T>(string key);

    Task SaveSettingAsync<T>(string key, T value);

    /// <summary>
    /// Writes several settings in one pass, synchronously. Shutdown paths must use this: an awaited
    /// continuation is abandoned when the process exits, so the file never gets written.
    /// </summary>
    void SaveSettings(IReadOnlyCollection<KeyValuePair<string, object>> values);
}
