using System.Globalization;
using assert;

namespace Date;

public class DateUtils
{
    public const string DefaultDateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    
    public const string DefaultDateFormat = "yyyy-MM-dd";
    
    public const string StandardJsonDateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    
    // ISO 8601 format
    public const string RequestDateTimeFormat = "O";

    public static string DateTimeToStandardJsonFormat(DateTime dateTime)
        => FormatDateTime(dateTime, StandardJsonDateTimeFormat);

    public static DateTime StandardJsonFormatToDateTime(string dateTime)
        => ParseDateTime(dateTime, StandardJsonDateTimeFormat);
    
    public static string GetCurrentDateAsString(string format = DefaultDateFormat) 
        => FormatDateTime(DateTime.UtcNow, format); // Get the current UTC date and time
    
    public static string GetCurrentTimeAsString(string format = DefaultDateTimeFormat) 
        => FormatDateTime(DateTime.UtcNow, format); // Retrieve the current DateTime in UTC
    
    public static string FormatDateTime(DateTime dateTime, ReadOnlySpan<char> format)
    {
        // NOTE: We use 32 as the base buffer size for DateTime formatting
        // because some special formats like 'O' and 'F' have format strings of length 1 but can output longer strings.
        // The buffer size is dynamically adjusted for longer format strings.
        int bufSize = format.Length > 32 ? format.Length : 32;
        var buffer = new char[bufSize];
        Utils.Assert(dateTime.TryFormat(buffer, out int charsWritten, format), 
            $"Failed to format DateTime string with format: {format}");
        var result = new string(buffer, 0, charsWritten);
        return result;
    }
    
    public static DateTime ParseDateTime(string dateTime, ReadOnlySpan<char> format)
    {
        Utils.Assert(
            DateTime.TryParseExact(dateTime, format, null, DateTimeStyles.None,
                out DateTime result), $"Failed to parse DateTime string: {dateTime} with format: {format}");
        return result;
    }
}