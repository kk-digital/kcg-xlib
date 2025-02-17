#define DebugAssertLevel2

using System.Diagnostics;
using System.Runtime.CompilerServices;
using LogUtility;
namespace assert;

public class Utils
{
    public static void Assert(
        bool condition,
        string message = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string caller = null,
        [CallerFilePath] string filePath = null)
    {
        AssertInternal(condition, "", message, lineNumber, caller, filePath);
    }

    public static void Assert(
        string message = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string caller = null,
        [CallerFilePath] string filePath = null)
    {
        AssertInternal(false, "", message, lineNumber, caller, filePath);
    }

    public static void Assert(
        bool condition,
        string logFilter,
        string message,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string caller = null,
        [CallerFilePath] string filePath = null)
    {
        AssertInternal(condition, logFilter, message, lineNumber, caller, filePath);
    }

    private static void AssertInternal(
        bool condition, 
        string logFilter, 
        string message, 
        int lineNumber, 
        string caller,
        string filePath)
    {
#if ReleaseMode
            // empty for now
#elif DebugAssertLevel1
            // level 1 assertion does not crash the game
            // logs the error message and move on
            
            if (!condition)
            {
                // Combine all at the debug assert message
                string path = filePath.Remove(0, filePath.IndexOf("KCG"));
                LibLog.LogError(message + " " + path + "  at " + caller + "()" + "  line: " + lineNumber);
                throw new Exception();
            }

#elif DebugAssertLevel2
        if (condition) return;

        // Combine all at the debug assert message
        string path = filePath.Remove(0, filePath.IndexOf("kcg", StringComparison.OrdinalIgnoreCase));
        var logMessage = $"{message} {path} at {caller}() line: {lineNumber}";
        if (string.IsNullOrEmpty(logFilter))
            LibLog.LogError(logMessage);
        else
            LibLog.LogError(logFilter, logMessage);
        Debug.Assert(condition, message);
        Debug.Fail("Execution Failed");
        throw new Exception(logMessage);
#endif
    }
}