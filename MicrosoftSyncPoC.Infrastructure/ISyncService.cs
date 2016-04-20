using System;
using System.Data;
using System.Runtime.Serialization;
using System.ServiceModel;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;

namespace MicrosoftSyncPoC.Infrastructure
{
    [ServiceContract(SessionMode = SessionMode.Required)] // Important attribute
    [ServiceKnownType(typeof (SyncIdFormatGroup))]
    [ServiceKnownType(typeof (DbSyncContext))]
    [ServiceKnownType(typeof (SyncSchema))]
    [ServiceKnownType(typeof (WebSyncFaultException))]
    [ServiceKnownType(typeof (SyncBatchParameters))]
    [ServiceKnownType(typeof (GetChangesParameters))]
    public interface ISyncService
    {
        // Defines the first method to call when using SessionMode
        [OperationContract(IsInitiating = true)]
        void Initialize(string scopeName);

        [OperationContract]
        DbSyncScopeDescription GetScopeDescription();

        [OperationContract]
        void BeginSession(SyncProviderPosition position);

        [OperationContract]
        SyncBatchParameters GetKnowledge();

        [OperationContract]
        GetChangesParameters GetChanges(uint batchSize,
            SyncKnowledge destinationKnowledge);

        [OperationContract]
        SyncSessionStatistics ApplyChanges(ConflictResolutionPolicy
            resolutionPolicy, ChangeBatch sourceChanges, object changeData);

        [OperationContract]
        bool HasUploadedBatchFile(string batchFileid, string remotePeerId);

        [OperationContract]
        void UploadBatchFile(string batchFileid, byte[] batchFile,
            string remotePeerId);

        [OperationContract]
        byte[] DownloadBatchFile(string batchFileId);

        [OperationContract]
        void EndSession();

        //Indicates the last method to call when use SessionMode
        [OperationContract(IsTerminating = true)]
        void Cleanup();
    }

    [DataContract]
    public class SyncBatchParameters
    {
        [DataMember] public uint BatchSize;
        [DataMember] public SyncKnowledge DestinationKnowledge;
    }

    [DataContract]
    [KnownType(typeof (DataSet))]
    public class GetChangesParameters
    {
        [DataMember] public ChangeBatch ChangeBatch;
        [DataMember] public object DataRetriever;
    }

    [DataContract]
    public class WebSyncFaultException
    {
        public Exception innerException;
        public string message;

        public WebSyncFaultException(string message, Exception innerException)
        {
            this.message = message;
            this.innerException = innerException;
        }

        [DataMember]
        public string Message
        {
            get { return message; }

            set { message = value; }
        }

        [DataMember]
        public Exception InnerException
        {
            get { return innerException; }

            set { innerException = value; }
        }
    }
}