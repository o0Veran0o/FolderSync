using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

public class FolderSynchronizer
{
    private readonly string sourcePath;
    private readonly string replicaPath;
    private readonly Logger logger;

    public FolderSynchronizer(string sourcePath, string replicaPath, Logger logger)
    {
        this.sourcePath = sourcePath;
        this.replicaPath = replicaPath;
        this.logger = logger;
    }

    public void Synchronize()
    {
        logger.Log("--- Synchronization Started ---");

        var sourceDirectory = new DirectoryInfo(sourcePath);
        var replicaDirectory = new DirectoryInfo(replicaPath);

        if (!sourceDirectory.Exists)
        {
            logger.Log($"Error: Source directory not found at '{sourcePath}'.");
            return;
        }

        // Ensure replica root exists
        if (!replicaDirectory.Exists)
        {
            Directory.CreateDirectory(replicaPath);
            logger.Log($"Created replica directory: '{replicaPath}'");
        }

        //  Copy/Update files and directories from Source to Replica
        SyncDirectoryRecursively(sourceDirectory, replicaDirectory);

        //  Remove files and directories from Replica that are not in Source
        CleanReplicaRecursively(sourceDirectory, replicaDirectory);

        logger.Log("--- Synchronization Finished ---");
    }


    private void SyncDirectoryRecursively(DirectoryInfo source, DirectoryInfo replica)
    {
        // Sync sub-directories
        foreach (var sourceSubDir in source.GetDirectories())
        {
            var replicaSubDir = new DirectoryInfo(Path.Combine(replica.FullName, sourceSubDir.Name));
            if (!replicaSubDir.Exists)
            {
                replicaSubDir.Create();
                logger.Log($"Created directory: '{replicaSubDir.FullName}'");
            }
            SyncDirectoryRecursively(sourceSubDir, replicaSubDir);
        }

        // Sync files
        foreach (var sourceFile in source.GetFiles())
        {
            string replicaFilePath = Path.Combine(replica.FullName, sourceFile.Name);
            var replicaFile = new FileInfo(replicaFilePath);

            if (!replicaFile.Exists || AreFilesDifferent(sourceFile, replicaFile))
            {
                sourceFile.CopyTo(replicaFilePath, true);
                logger.Log($"Copied file: '{sourceFile.FullName}' to '{replicaFilePath}'");
            }
        }
    }

    private void CleanReplicaRecursively(DirectoryInfo source, DirectoryInfo replica)
    {
        // Clean sub-directories
        foreach (var replicaSubDir in replica.GetDirectories())
        {
            var sourceSubDir = new DirectoryInfo(Path.Combine(source.FullName, replicaSubDir.Name));
            if (!sourceSubDir.Exists)
            {
                replicaSubDir.Delete(true); // true for recursive delete
                logger.Log($"Deleted directory: '{replicaSubDir.FullName}'");
            }
            else
            {
                CleanReplicaRecursively(sourceSubDir, replicaSubDir);
            }
        }

        // Clean files
        foreach (var replicaFile in replica.GetFiles())
        {
            string sourceFilePath = Path.Combine(source.FullName, replicaFile.Name);
            if (!File.Exists(sourceFilePath))
            {
                replicaFile.Delete();
                logger.Log($"Deleted file: '{replicaFile.FullName}'");
            }
        }
    }

  
    private bool AreFilesDifferent(FileInfo sourceFile, FileInfo replicaFile)
    {
        // Different size means they are different
        if (sourceFile.Length != replicaFile.Length)
        {
            return true;
        }

        // Same size, compare content using MD5 hash
        using (var md5 = MD5.Create())
        {
            using (var sourceStream = File.OpenRead(sourceFile.FullName))
            using (var replicaStream = File.OpenRead(replicaFile.FullName))
            {
                var sourceHash = md5.ComputeHash(sourceStream);
                var replicaHash = md5.ComputeHash(replicaStream);

                return !sourceHash.SequenceEqual(replicaHash);
            }
        }
    }
}