namespace TecWrapperApi.Utils;

internal enum LogLevel
{
    Info,
    Warning,
    Error
}

internal class Logger
{
    private bool isDisabled;

    internal Logger(bool disable = false)
    {
        this.isDisabled = disable;
    }
    
    internal void Log(LogLevel level, string message)
    {
        if(!this.isDisabled)
            Console.WriteLine($"[{level.ToString()}]: {message}");
    }
}