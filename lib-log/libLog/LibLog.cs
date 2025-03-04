using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace LogUtility
{
    public static class LibLog
    {
        
        private static object _lock = new object();
        
        // log saver
        public static Dictionary<string, List<LibLogEntry>> DebugLogs = new();
        public static Dictionary<string, List<LibLogEntry>> InfoLogs = new();
        public static Dictionary<string, List<LibLogEntry>> WarningLogs = new();
        public static Dictionary<string, List<LibLogEntry>> ErrorLogs = new();
        public static Dictionary<string, List<LibLogEntry>> FatalLogs = new();

        // log color
        private static readonly ConsoleColor DebugColor = ConsoleColor.White;
        private static readonly ConsoleColor InfoColor = ConsoleColor.Cyan;
        private static readonly ConsoleColor WarningColor = ConsoleColor.Yellow;
        private static readonly ConsoleColor ErrorColor = ConsoleColor.Red;
        private static readonly ConsoleColor FatalColor = ConsoleColor.DarkRed;
        private static readonly ConsoleColor ProfileColor = ConsoleColor.Green;

        private static void WriteLog(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void AddLogEntry(Dictionary<string, List<LibLogEntry>> logDictionary, 
            string filter, string message, LogLevel level, 
            string project = null, string ns = null, string className = null, 
            string methodName = null, string filePath = null, int lineNumber = 0)
        {
            lock (_lock)
            {
                if (!logDictionary.ContainsKey(filter))
                {
                    logDictionary.Add(filter, new List<LibLogEntry>());
                }

                List<LibLogEntry> logsList = logDictionary[filter];
                DateTime currentTime = DateTime.UtcNow;

                LibLogEntry logEntry = new LibLogEntry
                {
                    Message = message,
                    LoggedAt = currentTime,
                    Level = level,
                    Project = project,
                    Namespace = ns,
                    ClassName = className,
                    MethodName = methodName,
                    FilePath = filePath,
                    LineNumber = lineNumber
                };

                logsList.Add(logEntry);
                
                ConsoleColor color = level switch
                {
                    LogLevel.Debug => DebugColor,
                    LogLevel.Info => InfoColor,
                    LogLevel.Warning => WarningColor,
                    LogLevel.Error => ErrorColor,
                    LogLevel.Fatal => FatalColor,
                    _ => DebugColor
                };
                
                string levelName = Enum.GetName(typeof(LogLevel), level);
                string projectInfo = string.IsNullOrEmpty(project) ? "" : $"{project}";
                string fileInfo = string.IsNullOrEmpty(filePath) ? "" : $"file:///{filePath} {lineNumber}";
                string classMethodInfo = string.IsNullOrEmpty(className) ? "" : $" - {className}.{methodName}";

                string logMessage = $"[{levelName}] {currentTime}: [{filter}] {message}, [{projectInfo} | {fileInfo} | {classMethodInfo}]";
                WriteLog(logMessage, color);

            }
        }

        // info helper
        private static (string project,string fileName, string ns, string className) GetCallerInfo(string callerFilePath, string callerMemberName)
        {
            try
            {
                // get file name
                string fileName = Path.GetFileName(callerFilePath);
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = "Unknown";
                }
                // get project name (.csproj file name)
                string projectName = "Unknown";
                if (!string.IsNullOrEmpty(callerFilePath))
                {
                    string directory = Path.GetDirectoryName(callerFilePath);
                    string[] csprojFiles = Directory.GetFiles(directory, "*.csproj", SearchOption.AllDirectories);
                    if (csprojFiles.Length > 0)
                    {
                        projectName = Path.GetFileNameWithoutExtension(csprojFiles[0]);
                    }
                    else
                    {
                        // if no csproj file found, use the directory name
                        projectName = new DirectoryInfo(directory).Name;
                    }
                }

                // get namespace and class name
                string className = "Unknown";
                string ns = "Unknown";
                StackTrace stackTrace = new StackTrace(2, false);
                if (stackTrace.FrameCount > 0)
                {
                    StackFrame frame = stackTrace.GetFrame(0);
                    MethodBase method = frame?.GetMethod();
                    if (method != null)
                    {
                        Type declaringType = method.DeclaringType;
                        if (declaringType != null)
                        {
                            className = declaringType.Name;
                            ns = declaringType.Namespace ?? "Unknown";
                        }
                    }
                }

                return (projectName,fileName, ns, className);
            }
            catch
            {
                return ("Unknown", "Unknown", "Unknown", "Unknown");
            }
        }


        public static void LogDetailedProfile(string msg, Stopwatch time, [CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            string timeString = time.ElapsedMilliseconds + "ms";
            string logMessage = $"[PROFILE]: {msg}, {timeString}, Time: {DateTime.Now}, File: {callerFilePath}: {callerLineNumber} - {callerName}";
            WriteLog(logMessage, ProfileColor);
        }
        
        public static void LogProfile(string msg, Stopwatch time)
        {
            string timeString = time.ElapsedMilliseconds + "ms";
            WriteLog(msg + " " + timeString, ProfileColor);
        }

        public static void LogRaw(string message)
        {
            WriteLog(message, DebugColor);
        }
        
        #region auto
        public static void LogDebug(string message, 
            [CallerMemberName] string callerName = "", 
            [CallerFilePath] string callerFilePath = "", 
            [CallerLineNumber] int callerLineNumber = 0)
        {
            callerFilePath = callerFilePath.Replace("\\", "/");
            var (project, fileName, ns, className) = GetCallerInfo(callerFilePath, callerName);
            AddLogEntry(DebugLogs, "Default", message, LogLevel.Debug, project, ns, className, callerName, callerFilePath, callerLineNumber);
        }

        public static void LogInfo(string message, 
            [CallerMemberName] string callerName = "", 
            [CallerFilePath] string callerFilePath = "", 
            [CallerLineNumber] int callerLineNumber = 0)
        {
            callerFilePath = callerFilePath.Replace("\\", "/");
            var (project, fileName, ns, className) = GetCallerInfo(callerFilePath, callerName);
            AddLogEntry(InfoLogs, "Default", message, LogLevel.Info, project, ns, className, callerName, callerFilePath, callerLineNumber);
        }

        public static void LogWarning(string message, 
            [CallerMemberName] string callerName = "", 
            [CallerFilePath] string callerFilePath = "", 
            [CallerLineNumber] int callerLineNumber = 0)
        {
            callerFilePath = callerFilePath.Replace("\\", "/");
            var (project, fileName, ns, className) = GetCallerInfo(callerFilePath, callerName);
            AddLogEntry(WarningLogs, "Default", message, LogLevel.Warning, project, ns, className, callerName, callerFilePath, callerLineNumber);
        }

        public static void LogError(string message, 
            [CallerMemberName] string callerName = "", 
            [CallerFilePath] string callerFilePath = "", 
            [CallerLineNumber] int callerLineNumber = 0)
        {
            callerFilePath = callerFilePath.Replace("\\", "/");
            var (project, fileName, ns, className) = GetCallerInfo(callerFilePath, callerName);
            AddLogEntry(ErrorLogs, "Default", message, LogLevel.Error, project, ns, className, callerName, callerFilePath, callerLineNumber);
        }

        public static void LogFatal(string message, 
            [CallerMemberName] string callerName = "", 
            [CallerFilePath] string callerFilePath = "", 
            [CallerLineNumber] int callerLineNumber = 0)
        {
            callerFilePath = callerFilePath.Replace("\\", "/");
            var (project, fileName, ns, className) = GetCallerInfo(callerFilePath, callerName);
            AddLogEntry(FatalLogs, "Default", message, LogLevel.Fatal, project, ns, className, callerName, callerFilePath, callerLineNumber);
        }
        #endregion

        #region auto with filter
        public static void LogDebugWithFilter(string filter, string message, 
            [CallerMemberName] string callerName = "", 
            [CallerFilePath] string callerFilePath = "", 
            [CallerLineNumber] int callerLineNumber = 0)
        {
            callerFilePath = callerFilePath.Replace("\\", "/");
            var (project, fileName, ns, className) = GetCallerInfo(callerFilePath, callerName);
            AddLogEntry(DebugLogs, filter, message, LogLevel.Debug, project, ns, className, callerName, callerFilePath, callerLineNumber);
        }
        
        public static void LogInfoWithFilter(string filter, string message, 
            [CallerMemberName] string callerName = "", 
            [CallerFilePath] string callerFilePath = "", 
            [CallerLineNumber] int callerLineNumber = 0)
        {
            callerFilePath = callerFilePath.Replace("\\", "/");
            var (project, fileName, ns, className) = GetCallerInfo(callerFilePath, callerName);
            AddLogEntry(InfoLogs, filter, message, LogLevel.Info, project, ns, className, callerName, callerFilePath, callerLineNumber);
        }
        
        public static void LogWarningWithFilter(string filter, string message, 
            [CallerMemberName] string callerName = "", 
            [CallerFilePath] string callerFilePath = "", 
            [CallerLineNumber] int callerLineNumber = 0)
        {
            callerFilePath = callerFilePath.Replace("\\", "/");
            var (project, fileName, ns, className) = GetCallerInfo(callerFilePath, callerName);
            AddLogEntry(WarningLogs, filter, message, LogLevel.Warning, project, ns, className, callerName, callerFilePath, callerLineNumber);
        }
        
        public static void LogErrorWithFilter(string filter, string message, 
            [CallerMemberName] string callerName = "", 
            [CallerFilePath] string callerFilePath = "", 
            [CallerLineNumber] int callerLineNumber = 0)
        {
            callerFilePath = callerFilePath.Replace("\\", "/");
            var (project, fileName, ns, className) = GetCallerInfo(callerFilePath, callerName);
            AddLogEntry(ErrorLogs, filter, message, LogLevel.Error, project, ns, className, callerName, callerFilePath, callerLineNumber);
        }
        
        public static void LogFatalWithFilter(string filter, string message, 
            [CallerMemberName] string callerName = "", 
            [CallerFilePath] string callerFilePath = "", 
            [CallerLineNumber] int callerLineNumber = 0)
        {
            callerFilePath = callerFilePath.Replace("\\", "/");
            var (project, fileName, ns, className) = GetCallerInfo(callerFilePath, callerName);
            AddLogEntry(FatalLogs, filter, message, LogLevel.Fatal, project, ns, className, callerName, callerFilePath, callerLineNumber);
        }
        #endregion
        
        #region manual
        public static void LogDebugManual(string filter, string message, string project, string ns, 
            string className, string methodName, string filePath, int lineNumber)
        {
            AddLogEntry(DebugLogs, filter, message, LogLevel.Debug, project, ns, className, methodName, filePath, lineNumber);
        }
        
        public static void LogInfoManual(string filter, string message, string project, string ns, 
            string className, string methodName, string filePath, int lineNumber)
        {
            AddLogEntry(InfoLogs, filter, message, LogLevel.Info, project, ns, className, methodName, filePath, lineNumber);
        }
        
        public static void LogWarningManual(string filter, string message, string project, string ns, 
            string className, string methodName, string filePath, int lineNumber)
        {
            AddLogEntry(WarningLogs, filter, message, LogLevel.Warning, project, ns, className, methodName, filePath, lineNumber);
        }
        
        public static void LogErrorManual(string filter, string message, string project, string ns, 
            string className, string methodName, string filePath, int lineNumber)
        {
            AddLogEntry(ErrorLogs, filter, message, LogLevel.Error, project, ns, className, methodName, filePath, lineNumber);
        }
        
        public static void LogFatalManual(string filter, string message, string project, string ns, 
            string className, string methodName, string filePath, int lineNumber)
        {
            AddLogEntry(FatalLogs, filter, message, LogLevel.Fatal, project, ns, className, methodName, filePath, lineNumber);
        }
        #endregion

    }
}
