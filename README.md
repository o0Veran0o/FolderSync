How to Compile and Run:
Place all three files (Program.cs, Logger.cs, and FolderSynchronizer.cs) in your project directory.
Build the project:
dotnet build -c Release

Navigate to the output directory:
cd bin/Release/net8.0  

Run the program from the command line, providing the four required arguments.
Example Usage:
Create some test folders first:
C:\SyncTest\Source
C:\SyncTest\Replica
C:\SyncTest\Logs

Then run the command:
dotnet FolderSync.dll "C:\SyncTest\Source" "C:\SyncTest\Replica" 10 "C:\SyncTest\Logs\log.txt"

This command will:
Synchronize from C:\SyncTest\Source to C:\SyncTest\Replica.
Perform the synchronization check every 10 seconds.
Write all logs to C:\SyncTest\Logs\log.txt.
