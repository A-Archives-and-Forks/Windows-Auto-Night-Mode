using Microsoft.Extensions.Options;

namespace AutoDarkModeApp.Services;

public class LocalSettingsService : ILocalSettingsService
{
    private const string _defaultApplicationDataFolder = "AutoDarkMode/ApplicationData";
    private const string _defaultLocalSettingsFile = "LocalSettings.json";

    private readonly IFileService _fileService;
    private readonly LocalSettingsOptions _options;

    private readonly string _localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    private readonly string _applicationDataFolder;
    private readonly string _localsettingsFile;

    // Every setter mutates this one dictionary and rewrites the whole file, so all access is gated.
    private readonly SemaphoreSlim _gate = new(1, 1);

    private IDictionary<string, object> _settings;

    private bool _isInitialized;

    public LocalSettingsService(IFileService fileService, IOptions<LocalSettingsOptions> options)
    {
        _fileService = fileService;
        _options = options.Value;

        _applicationDataFolder = Path.Combine(_localApplicationData, _options.ApplicationDataFolder ?? _defaultApplicationDataFolder);
        _localsettingsFile = _options.LocalSettingsFile ?? _defaultLocalSettingsFile;

        _settings = new Dictionary<string, object>();
    }

    private void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        _settings = _fileService.Read<IDictionary<string, object>>(_applicationDataFolder, _localsettingsFile) ?? new Dictionary<string, object>();
        _isInitialized = true;
    }

    private async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        _settings = await Task.Run(() => _fileService.Read<IDictionary<string, object>>(_applicationDataFolder, _localsettingsFile)) ?? new Dictionary<string, object>();
        _isInitialized = true;
    }

    public async Task<T?> ReadSettingAsync<T>(string key)
    {
        await InitializeAsync();

        await _gate.WaitAsync();
        try
        {
            if (_settings.TryGetValue(key, out var obj))
            {
                return Json.ToObject<T>(obj.ToString()!);
            }
            return default;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task SaveSettingAsync<T>(string key, T value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        await InitializeAsync();

        // Deliberately synchronous from here on. Once the settings are loaded this method completes
        // without ever yielding, so the value is on disk even when the caller does not await it.
        SaveSettings([new KeyValuePair<string, object>(key, value)]);
    }

    public void SaveSettings(IReadOnlyCollection<KeyValuePair<string, object>> values)
    {
        if (values.Count == 0)
        {
            return;
        }

        Initialize();

        _gate.Wait();
        try
        {
            foreach (var setting in values)
            {
                _settings[setting.Key] = Json.Stringify(setting.Value);
            }
            _fileService.Save(_applicationDataFolder, _localsettingsFile, _settings);
        }
        finally
        {
            _gate.Release();
        }
    }
}
