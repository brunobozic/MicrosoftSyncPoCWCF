using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServer;
using MicrosoftSyncPoC.Client.WPF.Properties;

namespace MicrosoftSyncPoC.Client.WPF
{
    public class SyncResults
    {
        public SyncResults(string message,
            SyncOperationStatistics stats)
        {
            Message = message;
            Stats = stats;
        }

        public SyncOperationStatistics Stats { get; set; }
        public string Message { get; set; }
    }

    public class SynchronizationHelper
    {
        public void SynchronizeAsync(
            BackgroundWorker worker,
            DoWorkEventArgs e)
        {
            SyncResults results;
            results = SynchronizeProductsEmployees();
            worker.ReportProgress(0, results);
        }

        public SyncResults SynchronizeProductsEmployees()
        {
            SyncResults results = null;
            // Create the SQL CE Sync Provider for the given scope name


            var localProvider =
                ConfigureSQLSyncProvider("NorthwindServerSyncScope");


            // Create the remote provider for the given scope name
            var destinationProxy =
                new RelationalProviderProxy("NorthwindServerSyncScope",
                    Settings.Default.ServiceUrl);
            // Synchronize and collect results
            results = new SyncResults("NorthwindServerSyncScope",
                SynchronizeProviders(localProvider, destinationProxy,
                    SyncDirectionOrder.DownloadAndUpload));
            destinationProxy.Dispose();
            localProvider.Dispose();
            return results;
        }

        /// <summary>
        ///     Utility function that configures a CE provider
        /// </summary>
        /// <param name="sqlCeConnection"></param>
        /// <returns></returns>
        private SqlSyncProvider ConfigureSQLSyncProvider(string scopeName)
        {
            var sqlCeConnection = new SqlConnection(
                Settings.Default.DbConnection);
            var provider = new SqlSyncProvider();
            //Set the scope name
            provider.ScopeName = scopeName;

            //Set the connection.
            provider.Connection = sqlCeConnection;
            provider.ObjectSchema = "dbo";
            //Thats it. We are done configuring the CE provider.
            return provider;
        }

        /// <summary>
        ///     Utility function that will create a SyncOrchestrator and
        ///     synchronize the two passed in providers
        /// </summary>
        /// <param name="localProvider">Local store provider</param>
        /// <param name="remoteProvider">Remote store provider</param>
        /// <returns></returns>
        private SyncOperationStatistics SynchronizeProviders(
            KnowledgeSyncProvider localProvider, KnowledgeSyncProvider remoteProvider,
            SyncDirectionOrder direction)
        {
            var orchestrator = new SyncOrchestrator();
            orchestrator.LocalProvider = localProvider;
            orchestrator.RemoteProvider = remoteProvider;
            orchestrator.Direction = direction;

            // subscribe for errors that occur when applying changes to the client
            // ((SqlSyncProvider) orchestrator.RemoteProvider).ApplyChangeFailed += Program_ApplyChangeFailed;
            ((SqlSyncProvider) orchestrator.LocalProvider).ChangesSelected += Program_ChangesSelected;
            ((SqlSyncProvider) orchestrator.LocalProvider).SyncProgress += Program_LocalProgress;
            ((SqlSyncProvider) orchestrator.LocalProvider).ApplyChangeFailed += Program_ApplyChangeFailed;
            ((SqlSyncProvider) orchestrator.LocalProvider).ApplyingChanges += Program_ApplyingChanges;
            ((SqlSyncProvider) orchestrator.LocalProvider).ChangesApplied += Program_ChangesApplied;
            ((RelationalProviderProxy) orchestrator.RemoteProvider).DestinationCallbacks.ItemConflicting +=
                Program_RemoteItemConflicting;
            ((RelationalProviderProxy) orchestrator.RemoteProvider).DestinationCallbacks.ItemChanging +=
                Program_RemoteItemChanging;
            ((RelationalProviderProxy) orchestrator.RemoteProvider).DestinationCallbacks.ProgressChanged +=
                Program_ProgressChange;
            ((RelationalProviderProxy) orchestrator.RemoteProvider).DestinationCallbacks.ItemChangeSkipped +=
                Program_ItemChangeSkipped;

            ((SqlSyncProvider) orchestrator.LocalProvider).MemoryDataCacheSize = 100000;

            ((SqlSyncProvider) orchestrator.LocalProvider).ApplicationTransactionSize = 4096;


            //Check to see if any provider is a SqlCe provider and if it needs schema
            CheckIfProviderNeedsSchema(localProvider as SqlSyncProvider);
            var stats = orchestrator.Synchronize();
            return stats;
        }

        private static void Program_LocalProgress(object sender, DbSyncProgressEventArgs e)
        {
            // display conflict type
            Console.WriteLine(e.ScopeProgress.TotalChangesPending);
        }

        /// <summary>
        ///     Client processing - collating changes to send to the server
        ///     Fires before anything is "done", perfect spot to apply obscure business rules
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Program_ChangesSelected(object sender, DbChangesSelectedEventArgs e)
        {
            //let's check if we're synching the table we're interested
            if (e.Context.DataSet.Tables.Contains("Invoices"))
            {
                var dataTable = e.Context.DataSet.Tables["Invoices"];
                for (var j = 0; j < dataTable.Rows.Count; j++)
                {
                    var row = dataTable.Rows[j];

                    // we're only interested in updates
                    if (row.RowState == DataRowState.Modified)
                    {
                        // check if the status is Completed
                        if (Convert.ToInt64(row["Quantity"]) < 0)
                        {
                            // let's delete the row so it gets applied as a delete instead of applying it as an update
                            dataTable.Rows[j].Delete();
                        }
                    }
                }
            }
        }

        private static void Program_ItemChangeSkipped(object sender, ItemChangeSkippedEventArgs e)
        {
            // display conflict type
        }

        private static void Program_ProgressChange(object sender, SyncStagedProgressEventArgs e)
        {
            // display conflict type
            Console.WriteLine(e.TotalWork.ToString());
        }

        private static void Program_RemoteItemChanging(object sender, ItemChangingEventArgs e)
        {
            // display conflict type
            Console.WriteLine(e.Item.ChangeKind);
        }

        private static void Program_RemoteItemConflicting(object sender, ItemConflictingEventArgs e)
        {
            // display conflict type
            Console.WriteLine(e.SourceChange.ChangeKind.ToString());
        }

        private static void Program_ChangesApplied(object sender, DbChangesAppliedEventArgs e)
        {
            // display conflict type
            Console.WriteLine(e.Context.DataSet.DataSetName);
        }

        private static void Program_ApplyingChanges(object sender, DbApplyingChangesEventArgs e)
        {
            // display conflict type
            Console.WriteLine(e.Context.DataSet.DataSetName);
        }

        private static void Program_ApplyChangeFailed(object sender, DbApplyChangeFailedEventArgs e)
        {
            // display conflict type
            Console.WriteLine(e.Conflict.Type);
            Console.WriteLine(e.Conflict.Stage);

            // display error message 
            Console.WriteLine(e.Error);
        }

        /// <summary>
        ///     Check to see if the passed in CE provider needs Schema from server
        /// </summary>
        /// <param name="localProvider"></param>
        private void CheckIfProviderNeedsSchema(SqlSyncProvider localProvider)
        {
            if (localProvider != null)
            {
                var ceConn = (SqlConnection) localProvider.Connection;
                var ceConfig =
                    new SqlSyncScopeProvisioning(ceConn);
                ceConfig.ObjectSchema = "dbo";
                var scopeName = localProvider.ScopeName;

                //if the scope does not exist in this store
                if (!ceConfig.ScopeExists(scopeName))
                {
                    //create a reference to the server proxy
                    var serverProxy =
                        new RelationalProviderProxy(scopeName,
                            Settings.Default.ServiceUrl);

                    //retrieve the scope description from the server
                    var scopeDesc = serverProxy.GetScopeDescription();
                    serverProxy.Dispose();

                    //use scope description from server to intitialize the client
                    ceConfig.PopulateFromScopeDescription(scopeDesc);
                    ceConfig.Apply();
                }
            }
        }
    }
}