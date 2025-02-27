# LibLog Usage Guide

A concise and lightweight logging library that supports multi-level logging, automatic capture of caller information, and filter-based categorization.

## Log Levels

Supports five log levels:

- **Debug**: Used to output detailed information during development and debugging.
- **Info**: Used to record general information during program execution.
- **Warning**: Used to indicate events that may have potential issues.
- **Error**: Used to record errors that have occurred but do not prevent the program from continuing to run.
- **Fatal**: Used to record severe errors that cause the program to be unable to continue running.

## API Reference

### Basic Logging Methods (Automatic Caller Information Capture)

Parameters:
* message: Log message content
* callerName: Calling method name (automatically captured)
* callerFilePath: Calling file path (automatically captured)
* callerLineNumber: Calling line number (automatically captured)

```csharp
LibLog.LogDebug("test");
LibLog.LogInfo("test");
LibLog.LogWarning("test");
LibLog.LogError("test");
LibLog.LogFatal("test");
```

### Logging Methods with Filters

Parameters:
* filter: Log filter identifier (e.g., "YourFilter", "AnotherFilter", etc.)
* message: Log message content
* project: Project name (optional)
* ns: Namespace (optional)
* className: Class name (optional)
* methodName: Method name (optional)
* filePath: File path (optional)
* lineNumber: Line number (optional)

```csharp
LibLog.LogDebugWithFilter("UI", "Button clicked");
LibLog.LogInfoWithFilter("Database", "Data saved successfully");
LibLog.LogWarningWithFilter("Network", "Network connection unstable");
LibLog.LogErrorWithFilter("File System", "Unable to access file");
LibLog.LogFatalWithFilter("System", "Application crashed");
```

### Logging Methods with Fully Customized Parameters

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
LibLog.LogDebugWithFilter("filter", "message", "project", "namespace", "className", "methodName", "filePath", 1);
LibLog.LogInfoWithFilter("filter", "message", "project", "namespace", "className", "methodName", "filePath", 1);
LibLog.LogWarningWithFilter("filter", "message", "project", "namespace", "className", "methodName", "filePath", 1);
LibLog.LogErrorWithFilter("filter", "message", "project", "namespace", "className", "methodName", "filePath", 1);
LibLog.LogFatalWithFilter("filter", "message", "project", "namespace", "className", "methodName", "filePath", 1);
```

### Performance Logging Methods

Parameters for LogProfile:
* msg: Performance log message
* time: Stopwatch instance, used for timing

Parameters for LogDetailedProfile:
* msg: Performance log message
* time: Stopwatch instance, used for timing
* callerName: Calling method name (automatically captured)
* callerFilePath: Calling file path (automatically captured)
* callerLineNumber: Calling line number (automatically captured)

```csharp
// sw to get current time
Stopwatch sw = Stopwatch.StartNew();
// something time spanding...
sw.Stop();// get time length
LibLog.LogProfile("operation spand:", sw);

// Detailed performance logging (get more caller info)
Stopwatch sw2 = Stopwatch.StartNew();
// something time spanding...
sw2.Stop();
LibLog.LogDetailedProfile("complex task time catch", sw2);
```

## Terminal Output Examples

The following are terminal output examples for different types of logs:

```
// Basic log
[Debug] 2023-10-15 08:23:15.621: [Default] your message , [Project|Program.cs:15| - tester.basic]

// Log with filter
[Info] 2023-10-15 08:25:33.102: [Database] Data saved successfully, [Project|Program.cs:33|- tester.database]

// Log with fully customized parameters
[Warning] 2023-10-15 08:27:17.354: [Network] Timeout, [Project|Program.cs:89| - tester.network]

// Simple performance log
[PROFILE] operation spand: 502ms

// Detailed performance log
[PROFILE]: Complex calculation time, 803ms, Time: 2025-2-27 10:30:25.357, File: C:\Projects\test\test.cs: 125 - test23
```
