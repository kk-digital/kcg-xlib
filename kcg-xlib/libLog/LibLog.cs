using System.Diagnostics;
using System.Runtime.CompilerServices;
#if GODOT_BUILD
using Godot;
#endif

namespace LogUtility
{
    public static class LibLog
    {
        // Todo: Divide instrumantation and logging into channels
        
        private static int _verbosity = 0;
        private static char[] _profileBuffer = new char[8192];
        private static int _profileBufferIndex = 0;
        private static char[] _debugBuffer = new char[8192];
        private static int _debugBufferIndex = 0;
        private static char[] _wardingBuffer = new char[8192];
        private static int _warningBufferIndex = 0;
        private static char[] _errorBuffer = new char[8192];
        private static int _errorBufferIndex = 0;
        
        
        private static object _lock = new object();
        
        // Maps (filter) => (list of logs)
        public static Dictionary<string, List<LibLogEntry>> DebugLogs = new();
        // Maps (filter) => (list of logs)
        public static Dictionary<string, List<LibLogEntry>> WarningLogs = new();
        // Maps (filter) => (list of logs)
        public static Dictionary<string, List<LibLogEntry>> ErrorLogs = new();

        public static void LogDetailedProfile(string msg, Stopwatch time, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            string timeString = time.ElapsedMilliseconds + "ms";
            string logMessage = $"[PROFILE]: {msg}, {timeString}, Time: {DateTime.Now}, File: {callerFilePath}: {callerLineNumber} - {callerName}";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(logMessage);
            Console.ForegroundColor = ConsoleColor.White;
        }
        
        public static void LogProfile(string msg, Stopwatch time)
        {
            string timeString = time.ElapsedMilliseconds + "ms";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(msg + " " + timeString);
            Console.ForegroundColor = ConsoleColor.White;

        }
        
        public static void LogDetailedDebug(string msg, 
            [CallerMemberName] string callerName = "", 
            [CallerFilePath] string callerFilePath = "", 
            [CallerLineNumber] int callerLineNumber = 0)
        {
            string logMessage = $"[DEBUG]: {msg}, Time: {DateTime.Now}, File: {callerFilePath}: {callerLineNumber} - {callerName}";
            LogDebug(logMessage);
        }
    
        
        public static void LogDebug(string msg)
        {
#if GODOT_BUILD
            GD.Print(msg);
#else
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(msg);
#endif
        }
        
        public static void LogDetailedWarning(
            string msg, 
            [CallerMemberName] string callerName = "", 
            [CallerFilePath] string callerFilePath = "", 
            [CallerLineNumber] int callerLineNumber = 0)
        {
            string logMessage = $"[WARNING]: {msg}, Time: {DateTime.Now}, File: {callerFilePath}: {callerLineNumber} - {callerName}";
            LogWarning(logMessage);
        }
        
        public static void LogWarning(string msg)
        {
#if GODOT_BUILD
            GD.PushWarning(msg);
#else
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;
#endif
        }

        public static void LogDetailedError(
            string msg, 
            [CallerMemberName] string callerName = "", 
            [CallerFilePath] string callerFilePath = "", 
            [CallerLineNumber] int callerLineNumber = 0)
        {
            string logMessage = $"[ERROR]: {msg}, Time: {DateTime.Now}, File: {callerFilePath}: {callerLineNumber} - {callerName}";
            LogError(logMessage);
        }

        public static void LogError(string msg)
        {
#if GODOT_BUILD
            GD.PushError(msg);
#else
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;
#endif
        }   
        
        // Adding a new debug log entry with the provided filter and message
        public static void LogDebug(string filter, string msg)
        {
            lock (_lock)
            {
                // Check whether the filter exists in the dictionary
                if (!DebugLogs.ContainsKey(filter))
                {
                    // If the filter does not exist, create it
                    DebugLogs.Add(filter, new List<LibLogEntry>());
                }

                List<LibLogEntry> logsList = DebugLogs[filter];

                DateTime currentTime = DateTime.UtcNow;

                LibLogEntry logEntry = new LibLogEntry
                {
                    Message = msg,
                    LoggedAt = currentTime
                };

                logsList.Add(logEntry);

                string logMessage = $"Debug-{currentTime}: [{filter}]{msg}";
                LogDebug(logMessage);
            }
        }

        public static void LogWarning(string filter, string msg)
        {
            lock (_lock)
            {
                if (!WarningLogs.ContainsKey(filter))
                {
                    // If the filter does not exist, create it
                    WarningLogs.Add(filter, new List<LibLogEntry>());
                }

                List<LibLogEntry> logsList = WarningLogs[filter];

                DateTime currentTime = DateTime.UtcNow;

                LibLogEntry logEntry = new LibLogEntry
                {
                    Message = msg,
                    LoggedAt = currentTime
                };

                logsList.Add(logEntry);

                string logMessage = $"Warning-{currentTime}: [{filter}]{msg}";
                LogWarning(logMessage);
            }
        }

        public static void LogError(string filter, string msg)
        {
            lock (_lock)
            {
                if (!ErrorLogs.ContainsKey(filter))
                {
                    // If the filter does not exist, create it
                    ErrorLogs.Add(filter, new List<LibLogEntry>());
                }

                List<LibLogEntry> logsList = ErrorLogs[filter];

                DateTime currentTime = DateTime.UtcNow;

                LibLogEntry logEntry = new LibLogEntry
                {
                    Message = msg,
                    LoggedAt = currentTime
                };

                logsList.Add(logEntry);

                string logMessage = $"Error-{currentTime}: [{filter}] {msg}";
                LogError(logMessage);
            }
        }

        public static List<string> ListDebugLogs(string filter, DateTime startDate, DateTime endDate)
        {
            return new List<string>();
        }
    }
}
