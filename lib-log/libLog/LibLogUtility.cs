namespace LogUtility;

public static class LibLogUtility
{
    public static bool IsLogEnabled(LogLevel level, LibLogSettings settings)
    {
        switch (level)
        {
            case LogLevel.Debug:
            {
                return settings.EnableDebugLog;
            }
            case LogLevel.Info:
            {
                return settings.EnableInfoLog;
            }
            case LogLevel.Warning:
            {
                return settings.EnableWarningLog;
            }
            case LogLevel.Error:
            {
                return settings.EnableErrorLog;
            }
            case LogLevel.Fatal:
            {
                return settings.EnableFatalLog;
            }
            default:
            {
                return false;
            }
        }
    }
}