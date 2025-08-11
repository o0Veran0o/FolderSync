using System;
using System.IO;
using System.Threading;

public class Program
{
    private static Timer timer;
    private static FolderSynchronizer synchronizer;
    private static Logger logger;

    public static void Main(string[] args)
    {
        // Validate Arguments
        if (args.Length != 4)
        {
            Console.WriteLine("Usage: FolderSync.exe <SourcePath> <ReplicaPath> <SyncIntervalSeconds> <LogFilePath>");
            return;
        }

        string sourcePath = args[0];
        string replicaPath = args[1];
        if (!int.TryParse(args[2], out int syncIntervalSeconds))
        {
            Console.WriteLine("Error: Sync interval must be a valid integer (seconds).");
            return;
        }
        string logFilePath = args[3];

        // Ensure log file directory exists
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating log directory: {ex.Message}");
            return;
        }


        //  Initialize Logger 
        logger = new Logger(logFilePath);
        synchronizer = new FolderSynchronizer(sourcePath, replicaPath, logger);

        logger.Log("Synchronization service started.");
        logger.Log($"Source Path: {sourcePath}");
        logger.Log($"Replica Path: {replicaPath}");
        logger.Log($"Sync Interval: {syncIntervalSeconds} seconds");
        logger.Log($"Log File: {logFilePath}");

        // Set up the Timer 
        int syncIntervalMs = syncIntervalSeconds * 1000;
        timer = new Timer(SyncCallback, null, 0, syncIntervalMs);

        Console.WriteLine("Press [Enter] to exit.");
        Console.ReadLine();

        timer.Dispose();
        logger.Log("Synchronization service stopped.");
    }

    private static void SyncCallback(object state)
    {
        try
        {
            synchronizer.Synchronize();
        }
        catch (Exception ex)
        {
            logger.Log($"FATAL ERROR during synchronization: {ex.Message}");
        }
    }
}
