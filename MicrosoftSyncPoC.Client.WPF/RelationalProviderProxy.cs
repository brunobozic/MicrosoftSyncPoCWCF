using System;
using System.IO;
using System.ServiceModel;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;
using MicrosoftSyncPoC.Client.WPF.SyncService;
using SyncServiceInterface;


namespace MicrosoftSyncPoC.Client.WPF
{
    public class RelationalProviderProxy : KnowledgeSyncProvider, IDisposable
    {
        private string batchingDirectory =
            Environment.ExpandEnvironmentVariables("%TEMP%");

        protected bool disposed;
        protected SyncIdFormatGroup idFormatGroup;
        protected DirectoryInfo localBatchingDirectory;
        private ISyncService proxy;
        protected string scopeName;
        protected string serviceURL;

        public RelationalProviderProxy(string scopeName, string serviceURL)
        {
            this.scopeName = scopeName;
            this.serviceURL = serviceURL;
            CreateProxy();
            proxy.Initialize(scopeName);
        }

        public string BatchingDirectory
        {
            get { return batchingDirectory; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new
                        ArgumentException("value cannot be null or empty");
                }
                try
                {
                    var uri = new Uri(value);
                    if (!uri.IsFile || uri.IsUnc)
                    {
                        throw new
                            ArgumentException("value must be a local directory");
                    }
                    batchingDirectory = value;
                }
                catch (Exception e)
                {
                    throw new ArgumentException("Invalid batching directory.", e);
                }
            }
        }

        public override SyncIdFormatGroup IdFormats
        {
            get
            {
                if (idFormatGroup == null)
                {
                    idFormatGroup = new SyncIdFormatGroup();

                    //
                    //1 byte change unit id (Harmonica default before flexible ids)
                    //
                    idFormatGroup.ChangeUnitIdFormat.IsVariableLength = false;
                    idFormatGroup.ChangeUnitIdFormat.Length = 1;

                    //
                    // Guid replica id
                    //
                    idFormatGroup.ReplicaIdFormat.IsVariableLength = false;
                    idFormatGroup.ReplicaIdFormat.Length = 16;


                    //
                    // Sync global id for item ids
                    //
                    idFormatGroup.ItemIdFormat.IsVariableLength = true;
                    idFormatGroup.ItemIdFormat.Length = 10*1024;
                }

                return idFormatGroup;
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override void BeginSession(SyncProviderPosition position,
            SyncSessionContext syncSessionContext)
        {
            proxy.BeginSession(position);
        }

        public override void EndSession(SyncSessionContext syncSessionContext)
        {
            proxy.EndSession();
            if (localBatchingDirectory != null)
            {
                localBatchingDirectory.Refresh();

                if (localBatchingDirectory.Exists)
                {
                    //Cleanup batching releated files from this session
                    localBatchingDirectory.Delete(true);
                }
            }
        }

        public override ChangeBatch GetChangeBatch(uint batchSize,
            SyncKnowledge destinationKnowledge, out object changeDataRetriever)
        {
            var changesWrapper = proxy.GetChanges(batchSize,
                destinationKnowledge);
            //Retrieve the ChangeDataRetriever and the ChangeBatch
            changeDataRetriever = changesWrapper.DataRetriever;

            var context = changeDataRetriever as DbSyncContext;
            //Check to see if the data is batched.
            if (context != null && context.IsDataBatched)
            {
                if (localBatchingDirectory == null)
                {
                    //Retrieve the remote peer id from the 
                    //MadeWithKnowledge.ReplicaId. MadeWithKnowledge is the local  
                    //knowledge of the peer that is enumerating the changes.
                    var remotePeerId = context.MadeWithKnowledge.ReplicaId.ToString();

                    //Generate a unique Id for the directory.
                    //We use the peer id of the store enumerating the changes so 
                    //that the local temp directory is same for a given source
                    //across sync sessions. This enables us to restart a failed sync 
                    //by not downloading already received files.
                    var sessionDir =
                        Path.Combine(batchingDirectory, "WebSync_" + remotePeerId);
                    localBatchingDirectory = new DirectoryInfo(sessionDir);
                    //Create the directory if it doesnt exist.
                    if (!localBatchingDirectory.Exists)
                    {
                        localBatchingDirectory.Create();
                    }
                }

                var localFileName = Path.Combine(localBatchingDirectory.FullName,
                    context.BatchFileName);
                var localFileInfo = new FileInfo(localFileName);

                //Download the file only if doesnt exist                
                if (!localFileInfo.Exists)
                {
                    var remoteFileContents =
                        proxy.DownloadBatchFile(context.BatchFileName);
                    using (var localFileStream = new FileStream(localFileName,
                        FileMode.Create, FileAccess.Write))
                    {
                        localFileStream.Write(remoteFileContents, 0, remoteFileContents.Length);
                    }
                }
                //Set DbSyncContext.Batchfile name to the new local file name
                context.BatchFileName = localFileName;
            }

            return changesWrapper.ChangeBatch;
        }

        public override FullEnumerationChangeBatch
            GetFullEnumerationChangeBatch(uint batchSize, SyncId lowerEnumerationBound,
                SyncKnowledge knowledgeForDataRetrieval, out object changeDataRetriever)
        {
            throw new NotImplementedException();
        }

        public override void GetSyncBatchParameters(out uint batchSize,
            out SyncKnowledge knowledge)
        {
            var wrapper = proxy.GetKnowledge();
            batchSize = wrapper.BatchSize;
            knowledge = wrapper.DestinationKnowledge;
        }

        public override void ProcessChangeBatch(ConflictResolutionPolicy resolutionPolicy,
            ChangeBatch sourceChanges, object changeDataRetriever, SyncCallbacks syncCallbacks,
            SyncSessionStatistics sessionStatistics)
        {
            var context = changeDataRetriever as DbSyncContext;
            if (context != null && context.IsDataBatched)
            {
                var fileName = new FileInfo(context.BatchFileName).Name;

                //Retrieve the remote peer id from the MadeWithKnowledge.ReplicaId. 
                //MadeWithKnowledge is the local knowledge of the peer 
                //that is enumerating the changes.
                var peerId = context.MadeWithKnowledge.ReplicaId.ToString();

                //Check to see if service already has this file
                if (!proxy.HasUploadedBatchFile(fileName, peerId))
                {
                    //Upload this file to remote service
                    var stream = new FileStream(context.BatchFileName, FileMode.Open,
                        FileAccess.Read);
                    var contents = new byte[stream.Length];
                    using (stream)
                    {
                        stream.Read(contents, 0, contents.Length);
                    }
                    proxy.UploadBatchFile(fileName, contents, peerId);
                }

                context.BatchFileName = fileName;
            }

            var stats = proxy.ApplyChanges(resolutionPolicy,
                sourceChanges, changeDataRetriever);
            sessionStatistics.ChangesApplied += stats.ChangesApplied;
            sessionStatistics.ChangesFailed += stats.ChangesFailed;
        }

        public override void ProcessFullEnumerationChangeBatch(
            ConflictResolutionPolicy resolutionPolicy, FullEnumerationChangeBatch sourceChanges,
            object changeDataRetriever, SyncCallbacks syncCallbacks,
            SyncSessionStatistics sessionStatistics)
        {
            throw new NotImplementedException();
        }

        protected void CreateProxy()
        {
            var binding = new WSHttpBinding();
            //BasicHttpBinding binding = new BasicHttpBinding();
            binding.ReaderQuotas.MaxArrayLength = 999999999;
            binding.MaxReceivedMessageSize = 999999999;
            binding.Security.Mode = SecurityMode.None;
            binding.ReliableSession.Enabled = true;
            binding.SendTimeout = new TimeSpan(1,1,1,1);
            binding.ReceiveTimeout = new TimeSpan(1, 1, 1, 1);
         
            var factory =
                new ChannelFactory<ISyncService>(binding, new EndpointAddress(serviceURL));
            proxy = factory.CreateChannel();
        }

        public DbSyncScopeDescription GetScopeDescription()
        {
            return proxy.GetScopeDescription();
        }

        ~RelationalProviderProxy()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (proxy != null)
                    {
                        CloseProxy();
                    }
                }

                disposed = true;
            }
        }

        /// <summary>
        ///     Clean up and close proxy.
        /// </summary>
        public virtual void CloseProxy()
        {
            if (proxy != null)
            {
                proxy.Cleanup();
                var channel = proxy as ICommunicationObject;
                if (channel != null)
                {
                    try
                    {
                        channel.Close();
                    }
                    catch (TimeoutException)
                    {
                        channel.Abort();
                    }
                    catch (CommunicationException)
                    {
                        channel.Abort();
                    }
                }

                proxy = null;
            }
        }
    }
}