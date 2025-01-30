# CustomRunnerLib

*The modern and easiest Console Runner adapted to DI principles.*

Combine the power of Console apps with cancellation capability and custom logging. 

## How to install

Via tha Package Manager:
```powershell
Install-Package CustomRunnerLib
```

Via the .NET CLI
```bat
dotnet add package CustomRunnerLib
```

## How to use via DI

The common way to use the library is to add it to the `ServiceCollection` of the host builder:

```cs
IHost Host.CreateDefaultBuilder().ConfigureServices((context,services) =>
{
    services
        //we assume that the Driver will use the library (example given later)
        .AddScoped<Driver>() 
        //this adds the implementations for IConsoleRunner, IConsoleLogger
        .AddConsoleRunnerLib();
}).Build();
```

## IConsoleRunner

The `IConsoleRunner` contains exactly 3 methods to run a console application:

```cs
 public interface IConsoleRunner
 {
    //The events are used only when the RunWithEvents is called
     event EventHandler<DataReceivedEventArgs>? ErrorReceived;
     event EventHandler<DataReceivedEventArgs>? OutputReceived;

     Task<string?> Run(string executablePath, string arguments, string workingDirectory = "");
     Task<ProcessOutput> RunAndGetOutputAndError(string executablePath, string arguments, string workingDirectory = "");
     Task<int> RunWithEvents(string executablePath, string arguments, string workingDirectory = "", CancellationToken cancellationToken = default);
 }
```

In general a console application will output messages in both the standard output and standard error streams. 
If the output is expected to be minimal and we do not anticipate messages in the standard error stream, we can call the simple `Run` method.
In the example below, we aim to retrieve the title of a video file with the `yt-dlp` executable:

```cs
public class Driver
{
    private readonly IConsoleRunner _process;

    private readonly string _exePath = "c:/tools/yt-dlp.exe";
    private readonly ILogger<Driver> _logger;
    private readonly IServiceProvider _provider;

    public Driver(IConsoleRunner process, ILogger<Driver> logger, IServiceProvider provider)
    {
        _process = process;
        _logger = logger;
        _provider = provider;
    }


public async Task<string?> GetTitle(string videoId) =>
    await _process.Run(_exePath, $" --get-title -- {videoId}");
}
```

If we expect output for the standard error stream, then we call the more generic `RunAndGetOutputAndError` method. The returned `ProcessOutput` struct is returned which is as follows:

```cs
public readonly struct ProcessOutput
{
    public string StandardOutput { get; init; }
    public string StandardError { get; init; }
    public int ExitCode { get; init; }
}
```

Therefore we can inspect both the output and error streams, as well as the exit code of the console app, and act accordingly. For example:

```cs
    ProcessOutput output =  await _process.RunAndGetOutputAndError(_exePath, $" --get-title -- {videoId}");
    if(output.StandardError.Length > 0 )
        _logger.LogError(output.StandardError);
    else
        _logger.LogInformation(output.StandardOutput);
```

### Long runs

For long runs, the `RunWithEvents` provides full control capability over the application.

```cs
private async Task RunProcessExample()
{
    IConsoleRunner process =  _provider.GetConsoleRunner();
    process.OutputReceived += (object? s, DataReceivedEventArgs args) =>
    {
        string? status = args.Data;
        if (string.IsNullOrWhiteSpace(status)) return;
        
        //do something
        _logger.LogInformation(status);
    };

    process.ErrorReceived += (object? s, DataReceivedEventArgs args) =>
    {
        string? status = args.Data;
        if (string.IsNullOrWhiteSpace(status)) return;

        //do something
        _logger.LogError(status);
    };
    //...
    //we assume arguments and workingDirectory are defined before
    int exitCode = await process.RunWithEvents(_exePath, arguments, workingDirectory);
}
```

### Cancel a long run

Let's modify the example above in order to allow cancellation capability. We need to add a `CancellationToken` as the last parameter and handle the `TaskCancelledException` that will be thrown when the cancellation is called:

```cs
private async Task RunProcessExample(CancellationToken cancellationToken)
{
    IConsoleRunner process =  _provider.GetConsoleRunner();
    process.OutputReceived += (object? s, DataReceivedEventArgs args) =>
    {
    //...
    };

    process.ErrorReceived += (object? s, DataReceivedEventArgs args) =>
    {
    //...
    };
    //...
    try
    {
        //we assume arguments and workingDirectory are defined before
        int exitCode = await process.RunWithEvents(_exePath, arguments, workingDirectory, cancellationToken);
    }
    catch (TaskCancelledException)
    {
        _logger.LogInformation("Run was cancelled.")
    }

}
```

In order to initiate the cancellation, the cancellation token source should be accessible to both the method that will run the process and the method that will call the Cancel operation.

```cs
    CancellationTokenSource? cancelSource;

    async Task Runner()
    {
        //...
        cancelSource = new();
        await driver.RunProcessExample(cancelSource!.Token);
    }

    //this method will cause the TaskCancelledException to be thrown
    void CancelNow()
    {
        cancelSource?.Cancel();
    }
```
*STAY TUNED*

