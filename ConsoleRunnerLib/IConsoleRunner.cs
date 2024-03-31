using System.Diagnostics;

namespace ConsoleRunnerLib
{
    public interface IConsoleRunner
    {
        event EventHandler<DataReceivedEventArgs>? ErrorReceived;
        event EventHandler<DataReceivedEventArgs>? OutputReceived;

        Task<string?> Run(string executablePath, string arguments, string workingDirectory = "");
        Task<ProcessOutput> RunAndGetOutputAndError(string executablePath, string arguments, string workingDirectory = "");
        Task<int> RunWithEvents(string executablePath, string arguments, string workingDirectory = "", CancellationToken cancellationToken = default);
    }
}