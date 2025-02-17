
using OperatingSystemEnums;

namespace OperatingSystem;

public static class OperatingSystemUtils
{
    private static readonly OperatingSystemType OsType = GetOperatingSystemType();

    public static OperatingSystemType GetOperatingSystem()
    {
        return OsType;
    }

    private static OperatingSystemType GetOperatingSystemType()
    {
        if (System.OperatingSystem.IsWindows())
            return OperatingSystemType.Windows;
        if (System.OperatingSystem.IsLinux())
            return OperatingSystemType.Linux;
        if (System.OperatingSystem.IsMacOS())
            return OperatingSystemType.MacOS;
        
        return OperatingSystemType.Windows;
    }
}