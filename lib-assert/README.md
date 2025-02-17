# lib-assert

lib-assert is a C# library designed to facilitate assertions within your application. It enhances the debugging experience by providing detailed error information, including the file path, line number, and caller information when an assertion fails.

## Features

- **Multi-Level Debug Assertions**: The library supports different levels of debug assertions such as `DebugAssertLevel1` and `DebugAssertLevel2`, allowing the developer to control the severity of the assertions.
- **Custom Assertions**: Ability to create custom assertions which can include a custom message and log filter.
- **Caller Information**: Automatically captures caller information such as method name, file path, and line number, providing more context when an assertion fails.
- **Logging Integration**: Integrates with `LogUtility` for logging assertion failure messages.
- **Exception Throwing**: Automatically throws exceptions when an assertion fails.

## Usage

```csharp
using assert;

public class Example
{
    public void Test()
    {
        // Basic assertion without logging
        Utils.Assert(false, "Assertion failed!");

        // Assertion with logging filter and message
        Utils.Assert(false, "Filter", "Custom log message");

        // Assertion with custom message and automatic caller information
        Utils.Assert(false, "Caller information test");
    }
}
```

### Example of Assertion Failure

```csharp
Utils.Assert(false, "Filter", "Custom log message");
```

If `false` condition, the application will log an error message with all relevant information like file path, line number, caller, and message, and then throw an exception with that message.
