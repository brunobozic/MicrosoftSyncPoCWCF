using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServer;
using MicrosoftSyncPoC.Infrastructure;
using SyncService;

namespace MicrosoftSyncPoCWCF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    [AspNetCompatibilityRequirements(
        RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SyncService : ISyncService
    {
        private int batchCount;
        protected Dictionary<string, string> batchIdToFileMapper;
        protected bool isProxyToCompactDatabase;
        protected DirectoryInfo sessionBatchingDirectory;
        protected SqlSyncProvider sqlProvider;

        public void Initialize(string scopeName)
        {
            var helper = new ServerSynchronizationHelper();
            sqlProvider = helper.ConfigureSqlSyncProvider(scopeName);
            sqlProvider.ApplicationTransactionSize = 100000;
            sqlProvider.MemoryDataCacheSize = 4096;
            sqlProvider.ApplyingChanges += Program_RemoteItemChanging;
            sqlProvider.ChangesApplied += Program_ChangesApplied;
            sqlProvider.ApplyChangeFailed += Program_ChangeApplyFail;
            sqlProvider.ApplyMetadataFailed += Program_ApplyMetadataFail;
            sqlProvider.DestinationCallbacks.ItemConflicting += Program_ItemConflicting;
            sqlProvider.DestinationCallbacks.ItemChangeSkipped += Program_ItemChangeSkipped;

            // This is only for custom implemented providers...
            // sqlProvider.Configuration.CollisionConflictResolutionPolicy = CollisionConflictResolutionPolicy.ApplicationDefined;
            // sqlProvider.Configuration.ConflictResolutionPolicy = ConflictResolutionPolicy.ApplicationDefined;

            batchIdToFileMapper = new Dictionary<string, string>();
        }

        public DbSyncScopeDescription GetScopeDescription()
        {
            Log("GetSchema: {0}", sqlProvider.Connection.ConnectionString);

            var scopeDesc =
                SqlSyncDescriptionBuilder.GetDescriptionForScope(
                    sqlProvider.ScopeName, (SqlConnection) sqlProvider.Connection);
            return scopeDesc;
        }

        public void BeginSession(SyncProviderPosition position)
        {
            Log("*****************************************************************");
            Log("******************** New Sync Session ***************************");
            Log("*****************************************************************");
            Log("BeginSession: ScopeName: {0}, Position: {1}",
                sqlProvider.ScopeName, position);
            //Clean the mapper for each session.
            batchIdToFileMapper = new Dictionary<string, string>();

            sqlProvider.BeginSession(position, null /*SyncSessionContext*/);
            batchCount = 1024;
        }

        public SyncBatchParameters GetKnowledge()
        {
            Log("GetSyncBatchParameters: {0}", sqlProvider.Connection.ConnectionString);
            var destParameters = new SyncBatchParameters();
            sqlProvider.GetSyncBatchParameters(out destParameters.BatchSize,
                out destParameters.DestinationKnowledge);
            return destParameters;
        }

        public GetChangesParameters GetChanges(uint batchSize,
            SyncKnowledge destinationKnowledge)
        {
            Log("GetChangeBatch: {0}", sqlProvider.Connection.ConnectionString);
            var changesWrapper = new GetChangesParameters();
            changesWrapper.ChangeBatch = sqlProvider.GetChangeBatch(batchSize,
                destinationKnowledge, out changesWrapper.DataRetriever);

            var context = changesWrapper.DataRetriever as DbSyncContext;
            //Check to see if data is batched
            if (context != null && context.IsDataBatched)
            {
                Log("GetChangeBatch: Data Batched. Current Batch #:{0}", ++batchCount);
                //Dont send the file location info. Just send the file name
                var fileName = new FileInfo(context.BatchFileName).Name;
                batchIdToFileMapper[fileName] = context.BatchFileName;
                context.BatchFileName = fileName;
            }
            return changesWrapper;
        }

        public SyncSessionStatistics ApplyChanges(
            ConflictResolutionPolicy resolutionPolicy,
            ChangeBatch sourceChanges, object changeData)
        {
            Log("ProcessChangeBatch: {0}", sqlProvider.Connection.ConnectionString);

            var dataRetriever = changeData as DbSyncContext;

            if (dataRetriever != null && dataRetriever.IsDataBatched)
            {
                var remotePeerId = dataRetriever.MadeWithKnowledge.ReplicaId.ToString();
                //Data is batched. The client should have uploaded this file to us prior to 
                //calling ApplyChanges.
                //So look for it.
                //The Id would be the DbSyncContext.BatchFileName which is just the batch 
                //file name without the complete path
                string localBatchFileName = null;
                if (!batchIdToFileMapper.TryGetValue(dataRetriever.BatchFileName,
                    out localBatchFileName))
                {
                    //Service has not received this file. Throw exception
                    throw new FaultException<WebSyncFaultException>(
                        new WebSyncFaultException("No batch file uploaded for id " +
                                                  dataRetriever.BatchFileName,
                            null));
                }
                dataRetriever.BatchFileName = localBatchFileName;
            }

            var sessionStatistics = new SyncSessionStatistics();
            sqlProvider.ProcessChangeBatch(resolutionPolicy, sourceChanges,
                changeData,
                new SyncCallbacks(), sessionStatistics);
            return sessionStatistics;
        }

        public bool HasUploadedBatchFile(String batchFileId, string remotePeerId)
        {
            CheckAndCreateBatchingDirectory(remotePeerId);

            //The batchFileId is the fileName without the path information in it.
            var fileInfo = new FileInfo(Path.Combine(
                sessionBatchingDirectory.FullName,
                batchFileId));
            if (fileInfo.Exists && !batchIdToFileMapper.ContainsKey(batchFileId))
            {
                //If file exists but is not in the memory id to location mapper 
                //then add it to the mapping
                batchIdToFileMapper.Add(batchFileId, fileInfo.FullName);
            }
            //Check to see if the proxy has already uploaded this file to the service
            return fileInfo.Exists;
        }

        public void UploadBatchFile(string batchFileId, byte[] batchContents,
            string remotePeerId)
        {
            Log("UploadBatchFile: {0}", sqlProvider.Connection.ConnectionString);
            try
            {
                if (HasUploadedBatchFile(batchFileId, remotePeerId))
                {
                    //Service has already received this file. So dont save it again.
                    return;
                }

                //Service hasnt seen the file yet so save it.
                var localFileLocation = Path.Combine(
                    sessionBatchingDirectory.FullName, batchFileId);
                var fs = new FileStream(localFileLocation,
                    FileMode.Create, FileAccess.Write);
                using (fs)
                {
                    fs.Write(batchContents, 0, batchContents.Length);
                }
                //Save this Id to file location mapping in the mapper object
                batchIdToFileMapper[batchFileId] = localFileLocation;
            }
            catch (Exception e)
            {
                throw new FaultException<WebSyncFaultException>(
                    new WebSyncFaultException("Unable to save batch file.", e));
            }
        }

        public byte[] DownloadBatchFile(string batchFileId)
        {
            try
            {
                Log("DownloadBatchFile: {0}",
                    sqlProvider.Connection.ConnectionString);
                Stream localFileStream = null;

                string localBatchFileName = null;

                if (!batchIdToFileMapper.TryGetValue(batchFileId,
                    out localBatchFileName))
                {
                    throw new FaultException<WebSyncFaultException>(
                        new WebSyncFaultException("Unable to retrieve batch file for id."
                                                  + batchFileId, null));
                }

                using (localFileStream = new FileStream(localBatchFileName,
                    FileMode.Open, FileAccess.Read))
                {
                    var contents = new byte[localFileStream.Length];
                    localFileStream.Read(contents, 0, contents.Length);
                    return contents;
                }
            }
            catch (Exception e)
            {
                throw new FaultException<WebSyncFaultException>(
                    new WebSyncFaultException("Unable to read batch file for id " +
                                              batchFileId, e));
            }
        }

        public void EndSession()
        {
            Log("EndSession: {0}", sqlProvider.Connection.ConnectionString);
            Log("*****************************************************************");
            Log("******************** End Sync Session ***************************");
            Log("*****************************************************************");
            sqlProvider.EndSession(null);
            Log("");
        }

        public void Cleanup()
        {
            sqlProvider = null;
            //Delete all file in the temp session directory
            if (sessionBatchingDirectory != null)
            {
                sessionBatchingDirectory.Refresh();

                if (sessionBatchingDirectory.Exists)
                {
                    try
                    {
                        sessionBatchingDirectory.Delete(true);
                    }
                    catch
                    {
                        //Ignore 
                    }
                }
            }
        }

        private static void Program_ItemChangeSkipped(object sender, ItemChangeSkippedEventArgs e)
        {
        }

        /// <summary>
        ///     On the client (remote)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Program_ChangesApplied(object sender, DbChangesAppliedEventArgs e)
        {
        }

        private static void Program_ApplyMetadataFail(object sender, ApplyMetadataFailedEventArgs e)
        {
        }

        private static void Program_ItemConflicting(object sender, ItemConflictingEventArgs e)
        {
        }

        /// <summary>
        ///     On the client (remote)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Program_RemoteItemChanging(object sender, DbApplyingChangesEventArgs e)
        {
            // What is "remote item" in this case?

            var itemsAdded = e.Context.DataSet.GetChanges(DataRowState.Added);
            var itemsDeleted = e.Context.DataSet.GetChanges(DataRowState.Deleted);
            var itemsModified = e.Context.DataSet.GetChanges(DataRowState.Modified);
        }

        /// <summary>
        ///     On the client (remote)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Program_ChangeApplyFail(object sender, DbApplyChangeFailedEventArgs e)
        {
            // Summary:
            //     The peer database threw an exception while applying a change.
            // ErrorsOccurred = 0,
            //
            // Summary:
            //     The local and remote peers both updated the same row.
            // LocalUpdateRemoteUpdate = 1,
            //
            // Summary:
            //     The local peer updated a row that the remote peer deleted.
            // LocalUpdateRemoteDelete = 2,
            //
            // Summary:
            //     The local peer deleted a row that the remote peer updated.
            // LocalDeleteRemoteUpdate = 3,
            //
            // Summary:
            //     The local and remote peers both inserted a row that has the same primary
            //     key value. This caused a primary key violation.
            // LocalInsertRemoteInsert = 4,
            //
            // Summary:
            //     The local and remote peers both deleted the same row.
            // LocalDeleteRemoteDelete = 5,
            //
            // Summary:
            //     The local peer deleted a row that the remote peer updated, and the metadata
            //     for that row was cleaned up.
            // LocalCleanedupDeleteRemoteUpdate = 6,

            switch (e.Conflict.Type)
            {
                case DbConflictType.ErrorsOccurred:
                    var serverConflict = true;
                    var stage = e.Conflict.Stage;

                    if (e.Conflict.LocalChange != null)
                    {
                        serverConflict = true;
                    }
                    else if (e.Conflict.RemoteChange != null)
                    {
                        serverConflict = false;
                    }

                    // If we encounter an conflict while inserting server data into client database...
                    if (!serverConflict && stage == DbSyncStage.ApplyingInserts)
                    {
                        // since we want server to win insert conflicts...
                        if (e.Conflict.ErrorMessage.Contains("Violation of UNIQUE KEY constraint"))
                        {
                           // e.Action = ApplyAction.RetryWithForceWrite; // This wont work, constraint is a constraint
                        }

                        // In this case we need to forego changing/updating the root entity, BUT we want to be able to insert/update its children!!!
                        // Root Entity on Master remains the same, but the children taken from the client must be inserted and related to the root entity...


                    }


                    break;

                case DbConflictType.LocalCleanedupDeleteRemoteUpdate:

                    break;

                case DbConflictType.LocalDeleteRemoteDelete:

                    break;

                case DbConflictType.LocalDeleteRemoteUpdate:

                    break;

                case DbConflictType.LocalInsertRemoteInsert:
                    e.Action = ApplyAction.Continue;
                    break;

                case DbConflictType.LocalUpdateRemoteDelete:

                    break;

                case DbConflictType.LocalUpdateRemoteUpdate:

                    break;
            }

            // display conflict type
            Console.WriteLine(e.Conflict.ErrorMessage);
        }

        protected void Log(string p, params object[] paramArgs)
        {
            Console.WriteLine(p, paramArgs);
        }

        private void CheckAndCreateBatchingDirectory(string remotePeerId)
        {
            //Check to see if we have temp directory for this session.
            if (sessionBatchingDirectory == null)
            {
                //Generate a unique Id for the directory
                //We use the peer id of the store enumerating the changes so that the 
                //local temp directory is same for a given source
                //across sync sessions. This enables us to restart a failed sync by not 
                //downloading already received files.
                var sessionDir = Path.Combine(sqlProvider.BatchingDirectory,
                    "WebSync_" + remotePeerId);
                sessionBatchingDirectory = new DirectoryInfo(sessionDir);
                //Create the directory if it doesnt exist.
                if (!sessionBatchingDirectory.Exists)
                {
                    sessionBatchingDirectory.Create();
                }
            }
        }
    }
}