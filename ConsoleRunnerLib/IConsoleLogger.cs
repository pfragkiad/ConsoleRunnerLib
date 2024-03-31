
namespace ConsoleRunnerLib
{
    public interface IConsoleLogger
    {
        Task<int> RunWithEvents(string executablePath, string arguments, string workingDirectory = "", CancellationToken cancellationToken = default);
    }
}