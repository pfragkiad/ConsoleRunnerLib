namespace ConsoleRunnerLib;

public readonly struct ProcessOutput
{
    public string StandardOutput { get; init; }
    public string StandardError { get; init; }

    public int ExitCode { get; init; }
}
