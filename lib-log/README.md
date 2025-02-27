# LibLog Usage Guide

A concise and lightweight logging library that supports multi-level logging, automatic caller information retrieval, and filter-based categorization.

## Log Levels

Supports five log levels:

- **Debug**: Used to output detailed information during development and debugging.
- **Info**: Used to record general information about the program's execution.
- **Warning**: Used to indicate events that may have potential problems.
- **Error**: Used to record errors that have occurred but will not cause the program to stop running.
- **Fatal**: Used to record serious errors that cause the program to be unable to continue running.

## API Reference

### Basic Logging Methods (Automatic Caller Information)

Parameters:
* message: Log message content
* callerName: Calling method name (automatically obtained)
* callerFilePath: Calling file path (automatically obtained)
* callerLineNumber: Calling line number (automatically obtained)

```csharp
LibLog.LogDebug("test");
LibLog.LogInfo("test");
LibLog.LogWarning("test");
LibLog.LogError("test");
LibLog.LogFatal("test");
```

### Logging Methods with Filters (Automatic Caller Information)

Parameters:
* filter: Log filter identifier (e.g., "Youtube", "Minio", etc.)
* message: Log message content
* callerName: Calling method name (automatically obtained)
* callerFilePath: Calling file path (automatically obtained)
* callerLineNumber: Calling line number (automatically obtained)

```csharp
LibLog.LogDebugWithFilter("UI", "Button clicked");
LibLog.LogInfoWithFilter("Database", "Data saved successfully");
LibLog.LogWarningWithFilter("Network", "Network connection unstable");
LibLog.LogErrorWithFilter("File System", "Unable to access file");
LibLog.LogFatalWithFilter("System", "Application crashed");
```

### Logging Methods with Manually Specified Parameters

Parameters:
* filter: Log filter identifier
* message: Log message content
* project: Project name
* ns: Namespace
* className: Class name
* methodName: Method name
* filePath: File path
* lineNumber: Line number

```csharp
LibLog.LogDebugManual("filter", "message", "project", "namespace", "className", "methodName", "filePath", 1);
LibLog.LogInfoManual("filter", "message", "project", "namespace", "className", "methodName", "filePath", 1);
LibLog.LogWarningManual("filter", "message", "project", "namespace", "className", "methodName", "filePath", 1);
LibLog.LogErrorManual("filter", "message", "project", "namespace", "className", "methodName", "filePath", 1);
LibLog.LogFatalManual("filter", "message", "project", "namespace", "className", "methodName", "filePath", 1);
```

### Performance Logging Methods

LogProfile Parameters:
* msg: Performance log message
* time: Stopwatch instance for timing

LogDetailedProfile Parameters:
* msg: Performance log message
* time: Stopwatch instance for timing
* callerName: Calling method name (automatically obtained)
* callerFilePath: Calling file path (automatically obtained)
* callerLineNumber: Calling line number (automatically obtained)

```csharp
// Get the Stopwatch of the current time
Stopwatch sw = Stopwatch.StartNew();
// Execute time-consuming operations...
sw.Stop();// Get duration
LibLog.LogProfile("operation spand:", sw);

// Detailed performance log (get more caller information)
Stopwatch sw2 = Stopwatch.StartNew();
// Execute time-consuming operations...
sw2.Stop();
LibLog.LogDetailedProfile("complex task time catch", sw2);
```

## Terminal Output Example

The following are terminal output examples of different types of logs:

```
// Basic log
[Debug] 2023-10-15 08:23:15.621: [Default] your message , [Project|Program.cs:15| - tester.basic]

// Log with filter
[Info] 2023-10-15 08:25:33.102: [Database] Data saved successfully, [Project|Program.cs:33|- tester.database]

// Log with fully custom parameters
[Warning] 2023-10-15 08:27:17.354: [Network] Timeout, [Project|Program.cs:89| - tester.network]

// Simple performance log
[PROFILE] operation spand: 502ms

// Detailed performance log
[PROFILE]: Complex calculation time, 803ms, Time: 2025-2-27 10:30:25.357, File: C:\Projects\test\test.cs: 125 - test23
```
