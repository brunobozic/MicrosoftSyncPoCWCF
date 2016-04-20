using System;
using System.Data.SqlClient;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data.SqlServer;

namespace MicrosoftSyncPoC.ServerSide
{
    public class SyncStart
    {
        public static void Synchronize()
        {
            // Connection to  SQL Server
            var serverConn =
                new SqlConnection(@"Data Source=.\SQLEXPRESS; Initial Catalog=SyncDB; Trusted_Connection=Yes");

            // Connection to SQL client
            var clientConn =
                new SqlConnection(@"Data Source=.\SQLEXPRESS; Initial Catalog=SyncClientDB; Trusted_Connection=Yes");

            // Perform Synchronization between SQL Server and the SQL client.
            var syncOrchestrator = new SyncOrchestrator();

            // Create provider for SQL Server
            var serverProvider = new SqlSyncProvider("product", serverConn);

            // Set the command timeout and maximum transaction size for the SQL Azure provider.
            var clientProvider = new SqlSyncProvider("product", clientConn);

            // Set Local provider of SyncOrchestrator to the server provider
            syncOrchestrator.LocalProvider = serverProvider;

            // Set Remote provider of SyncOrchestrator to the client provider
            syncOrchestrator.RemoteProvider = clientProvider;

            // Set the direction of SyncOrchestrator session to Upload and Download
            syncOrchestrator.Direction = SyncDirectionOrder.UploadAndDownload;

            // Create SyncOperations Statistics Object
            var syncStats = syncOrchestrator.Synchronize();

            // Display the Statistics
            Console.WriteLine("Start Time: " + syncStats.SyncStartTime);
            Console.WriteLine("Total Changes Uploaded: " + syncStats.UploadChangesTotal);
            Console.WriteLine("Total Changes Downloaded: " + syncStats.DownloadChangesTotal);
            Console.WriteLine("Complete Time: " + syncStats.SyncEndTime);

            // Shut down database connections.
            serverConn.Close();
            serverConn.Dispose();
            clientConn.Close();
            clientConn.Dispose();
        }
    }
}