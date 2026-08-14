namespace AutoDarkModeApp.Contracts.Services;

public interface ICloseService
{
    /// <summary>
    /// Persists the window placement. Runs synchronously because it is called while the process is
    /// tearing down, where an async continuation is not guaranteed to run.
    /// </summary>
    void Close();
}
