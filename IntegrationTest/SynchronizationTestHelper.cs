using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using IntegrationTest.Properties;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServer;

namespace IntegrationTest
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

    public class SynchronizationTestHelper
    {
        public void SynchronizeAsync(
            BackgroundWorker worker,
            DoWorkEventArgs e)
        {
            SyncResults results;
            results = Synchronize();
            worker.ReportProgress(0, results);
        }

        public SyncResults Synchronize()
        {
            SyncResults results = null;
            // Create the SQL CE Sync Provider for the given scope name


            var localProvider =
                ConfigureSQLSyncProvider("NorthwindServerSyncConfig");

            // Create the remote provider for the given scope name
            var destinationProxy =
                new RelationalProviderTestProxy("NorthwindServerSyncConfig",
                    Settings.Default.ServiceUrl);
            destinationProxy.DestinationCallbacks.ItemConflicting += Program_ItemConflicting;
            destinationProxy.DestinationCallbacks.ItemConstraint += Program_ItemConstraint;

            // Synchronize and collect results
            results = new SyncResults("NorthwindServerSyncConfig",
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
                Settings.Default.northwind_guid_clientContext);
            var provider = new SqlSyncProvider();
            //Set the scope name
            provider.ScopeName = scopeName;

            //Set the connection.
            provider.Connection = sqlCeConnection;
            provider.ObjectSchema = "dbo";

            // This is only for custom implemented providers...
            //provider.Configuration.CollisionConflictResolutionPolicy = CollisionConflictResolutionPolicy.ApplicationDefined;
            //provider.Configuration.ConflictResolutionPolicy = ConflictResolutionPolicy.ApplicationDefined;

            return provider;
        }

        /// <summary>
        ///     Utility function that will create a SyncOrchestrator and
        ///     synchronize the two passed in providers
        /// </summary>
        /// <param name="localProvider">Local store provider</param>
        /// <param name="remoteProvider">Remote store provider</param>
        /// <param name="direction"></param>
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
            ((SqlSyncProvider) orchestrator.LocalProvider).ChangesSelected += Program_ChangesSelected;
            ((SqlSyncProvider) orchestrator.LocalProvider).SyncProgress += Program_LocalProgress;
            ((SqlSyncProvider) orchestrator.LocalProvider).ApplyChangeFailed += Program_ApplyChangeFailed;
            ((SqlSyncProvider) orchestrator.LocalProvider).ApplyingChanges += Program_ApplyingChanges;
            ((SqlSyncProvider) orchestrator.LocalProvider).ChangesApplied += Program_ChangesApplied;


            // These are used for file sync...
            //((RelationalProviderTestProxy) orchestrator.RemoteProvider).DestinationCallbacks.ItemConflicting +=
            //    Program_RemoteItemConflicting;
            //((RelationalProviderTestProxy) orchestrator.RemoteProvider).DestinationCallbacks.ItemChanging +=
            //    Program_RemoteItemChanging;
            //((RelationalProviderTestProxy) orchestrator.RemoteProvider).DestinationCallbacks.ProgressChanged +=
            //    Program_ProgressChange;
            //((RelationalProviderTestProxy) orchestrator.RemoteProvider).DestinationCallbacks.ItemChangeSkipped +=
            //    Program_ItemChangeSkipped;
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

        //private static void Program_ItemChangeSkipped(object sender, ItemChangeSkippedEventArgs e)
        //{
        //    // display conflict type
        //}

        //private static void Program_ProgressChange(object sender, SyncStagedProgressEventArgs e)
        //{
        //    // display conflict type
        //    Console.WriteLine(e.TotalWork.ToString());
        //}

        //private static void Program_RemoteItemChanging(object sender, ItemChangingEventArgs e)
        //{
        //    // display conflict type
        //    Console.WriteLine(e.Item.ChangeKind);
        //}

        //private static void Program_RemoteItemConflicting(object sender, ItemConflictingEventArgs e)
        //{
        //    // display conflict type
        //    Console.WriteLine(e.SourceChange.ChangeKind.ToString());
        //}


        private static void Program_ItemConstraint(object sender, ItemConstraintEventArgs e)
        {
            // display conflict type
            Console.WriteLine(e.SourceChange.ChangeKind.ToString());
        }

        private static void Program_ItemConflicting(object sender, ItemConflictingEventArgs e)
        {
            // display conflict type
            Console.WriteLine(e.SourceChange.ChangeKind.ToString());
        }

        /// <summary>
        ///     List of changes successfully applied to the client (taken from the server)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Program_ChangesApplied(object sender, DbChangesAppliedEventArgs e)
        {
            var added = e.Context.DataSet.GetChanges(DataRowState.Added);
            var deleted = e.Context.DataSet.GetChanges(DataRowState.Deleted);
            var modified = e.Context.DataSet.GetChanges(DataRowState.Modified);
        }

        /// <summary>
        ///     Appplying changes on the client (taken from the server)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Program_ApplyingChanges(object sender, DbApplyingChangesEventArgs e)
        {
            var added = e.Changes.GetChanges(DataRowState.Added);
            var deleted = e.Changes.GetChanges(DataRowState.Deleted);
            var modified = e.Changes.GetChanges(DataRowState.Modified);
        }

        private static void Program_ApplyChangeFailed(object sender, DbApplyChangeFailedEventArgs e)
        {
            // Both the server and the client have the same update on the same row, by default client (remote) wins
            if ((e.Conflict.Type == DbConflictType.LocalUpdateRemoteUpdate) &&
                (e.Conflict.Stage == DbSyncStage.ApplyingUpdates))
            {
                // If we use "ApplyAction.RetryWithForceWrite" the server will win the conflict...
                e.Action = ApplyAction.RetryWithForceWrite;


                // Otherwise (default behaviour), the client change will be written unto server (client wins)
            }
            else if ((e.Conflict.Type == DbConflictType.LocalInsertRemoteInsert) &&
                     (e.Conflict.Stage == DbSyncStage.ApplyingInserts))
            {
                // If we use "ApplyAction.RetryWithForceWrite" the server will win the conflict...
                e.Action = ApplyAction.RetryWithForceWrite;


                // Otherwise (default behaviour), the client change will be written unto server (client wins)
            }
            else if ((e.Conflict.Type == DbConflictType.LocalDeleteRemoteDelete) &&
                     (e.Conflict.Stage == DbSyncStage.ApplyingDeletes))
            {
                // If we use "ApplyAction.RetryWithForceWrite" the server will win the conflict...
                e.Action = ApplyAction.RetryWithForceWrite;


                // Otherwise (default behaviour), the client change will be written unto server (client wins)
            }
            else if ((e.Conflict.Type == DbConflictType.ErrorsOccurred) &&
                     (e.Conflict.Stage == DbSyncStage.ApplyingInserts))
            {
                #region Custom logic for proof of concept

                // Custom logic proof of concept (unique constraint)
                // Cannot insert duplicate key row in object 'dbo.Region' with unique index 'IX_RegionDescription'. The duplicate key value is (UniqueConstraintRegionDescription).
                // The statement has been terminated.
                if (e.Conflict.ErrorMessage.Contains("Cannot insert duplicate key row in object") &&
                    (e.Conflict.ErrorMessage.Contains("with unique index")))
                {
                    // For table named "Region" we want [server] to always win when the type of conflict is: unique name constraint...
                    // Additionally, all child relations from the [server] need to be written to the [client] database as well
                    // Additionally, client side conflicting row needs to be deleted BUT its child relations need to be updated to the 
                    // [server] value
                    if (e.Conflict.RemoteChange.TableName.Equals("Region"))
                    {
                        try
                        {
                            var conflictingUniqueConstraint = string.Empty;
                            var serverRegionId = Guid.Empty;
                       
                            if (e.Connection.State == ConnectionState.Open)
                            {
                                // 1. get the conflicting unique constraint from the [server]
                                if (e.Context.DataSet.Tables.Contains("Region"))
                                {
                                    var dataTable = e.Context.DataSet.Tables["Region"];
                                    for (var j = 0; j < dataTable.Rows.Count; j++)
                                    {
                                        var row = dataTable.Rows[j];


                                        // check if the status is Completed
                                        if (!string.IsNullOrEmpty(row["RegionDescription"].ToString()))
                                        {
                                            conflictingUniqueConstraint = row["RegionDescription"].ToString();
                                        }

                                        // get the unique Id of the conflicting row [server]
                                        serverRegionId = new Guid(row["RegionID"].ToString());
                                    }
                                }


                                // 2. Get the [client] entity id (Guid) for the conflicting row (which will be different from the [server]s)
                                var clientRegionId = string.Empty;

                                var sqlTextSel = "select * from dbo.Region where RegionDescription=@description ";
                                var myCommandSel = new SqlCommand(sqlTextSel, (SqlConnection) e.Connection,
                                    (SqlTransaction) e.Transaction);
                                myCommandSel.Parameters.AddWithValue("@description",
                                    conflictingUniqueConstraint.TrimEnd());

                                var dr = myCommandSel.ExecuteReader();
                                while (dr.Read())
                                {
                                    clientRegionId = dr["RegionID"].ToString();
                                }


                                // This should allow the [server] to insert its region data into the [client] database
                                 var sqlTextDeleteReg =
                                    "update dbo.Region set RegionDescription=@regionDescriptionConflicting where RegionID=@clientRegionId ";
                                 var myCommandDeleteReg = new SqlCommand(sqlTextDeleteReg, (SqlConnection)e.Connection,
                                    (SqlTransaction) e.Transaction);
                                 myCommandDeleteReg.Parameters.AddWithValue("@clientRegionId", clientRegionId);
                                 myCommandDeleteReg.Parameters.AddWithValue("@regionDescriptionConflicting", conflictingUniqueConstraint.TrimEnd() + "-Conflict-" + DateTime.Now);
                                 myCommandDeleteReg.ExecuteNonQuery();

								 //var sqlTextDeleteTer =
								 //  "update dbo.Territories set TerritoryDescription=territoryDescriptionConflicting where RegionID=@clientRegionId ";
								 //var myCommandDeleteTer = new SqlCommand(sqlTextDeleteTer, (SqlConnection)e.Connection,
								 //   (SqlTransaction)e.Transaction);
								 //myCommandDeleteTer.Parameters.AddWithValue("@clientRegionId", clientRegionId);
								 //myCommandDeleteTer.Parameters.AddWithValue("@territoryDescriptionConflicting", conflictingClientTerritory.TrimEnd() + "-Conflict-" + DateTime.Now);
								 //myCommandDeleteTer.ExecuteNonQuery();

                                //// var tempDataTable = new DataTable();
                                ////// create data adapter
                                //// var da = new SqlDataAdapter(myCommandSel);
                                ////// this will query your database and return the result to your datatable
                                //// var res = da.Fill(tempDataTable);
                                ////// var drc = tempDataTable.ParentRelations;
                                //// var x = tempDataTable.Rows[0].GetChildRows("RegionID");


                                //// 3. Every [client] side child entity related via the said guid, needs to be **updated** to [server] side guid...
                                //// need to start from the "childmost" and work our way up, in this extremely elaborate case, we only have the 
                                //// table named [Territory] that has a one to many relation to the [Region] table...no further tables are related 
                                //// making this a simple sample...
                                // var sqlTextDeleteTer =
                                //    "update dbo.Territory set RegionID=@serverRegionID where RegionID=@clientRegionId ";
                                // var myCommandDeleteTer = new SqlCommand(sqlTextDeleteTer, (SqlConnection) e.Connection,
                                //    (SqlTransaction) e.Transaction);
                                // myCommandDeleteTer.Parameters.AddWithValue("@clientRegionId", clientRegionId);
                                // myCommandDeleteTer.Parameters.AddWithValue("@serverRegionID", serverRegionId);
                                // myCommandDeleteTer.ExecuteNonQuery();


                                //// 4. delete the row that has the conflicting unique constraint - from the [local] database
                                //var sqlText = "delete from Region where RegionDescription=@description ";
                                //var myCommand = new SqlCommand(sqlText, (SqlConnection) e.Connection,
                                //    (SqlTransaction) e.Transaction);
                                //myCommand.Parameters.AddWithValue("@description", conflictingUniqueConstraint);

                                //myCommand.ExecuteNonQuery();
                            }


                            // 3. Let it sync (overwrite the client row with the server row
                            // If we use "ApplyAction.RetryWithForceWrite" the server will win the conflict...
                            e.Action = ApplyAction.RetryWithForceWrite;
                            // However the server is powerless against unique key constraint validation scenario...

                            e.Transaction.Commit();


                            // Problem: will client side entities that were just updated to feature server side guid be synced to the server?
                        }
                        catch (Exception regionExc)
                        {
                            e.Transaction.Rollback();
                        }
                    }
                }

                #endregion

                // Otherwise (default behaviour), the client change will be written unto server (client wins)

                if (e.Conflict.ErrorMessage.Contains("The INSERT statement conflicted with the FOREIGN KEY constraint"))
                {
                }
            }
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
                        new RelationalProviderTestProxy(scopeName,
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