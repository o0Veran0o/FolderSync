using System;
using System.IO;

public class Logger
{
    private readonly string logFilePath;
    private static readonly object @lock = new object();

    public Logger(string logFilePath)
    {
        this.logFilePath = logFilePath;
    }
    public void Log(string message)
    {
        string formattedMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";


        Console.WriteLine(formattedMessage);

        // Write to file with a lock to prevent race conditions
        lock (@lock)
        {
            try
            {
                File.AppendAllText(logFilePath, formattedMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // If logging to file fails, write error to console
                Console.WriteLine($"Could not write to log file {logFilePath}. Error: {ex.Message}");
            }
        }
    }
}