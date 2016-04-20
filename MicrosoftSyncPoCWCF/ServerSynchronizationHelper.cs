using System.Configuration;
using System.Data.SqlClient;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServer;

namespace SyncService
{
    public class ServerSynchronizationHelper
    {
        private readonly string conString =
            ConfigurationManager.ConnectionStrings["northwindContext"].ConnectionString;

        /// <summary>
        ///     Configure the SqlSyncprovider.  Note that this method assumes you have a direct
        ///     conection to the server as this is more of a design time use case vs. runtime
        ///     use case.  We think of provisioning the server as something that occurs before
        ///     an application is deployed whereas provisioning the client is somethng that
        ///     happens during runtime (on intitial sync) after the application is deployed.
        /// </summary>
        /// <param name="hostName"></param>
        /// <returns></returns>
        public SqlSyncProvider ConfigureSqlSyncProvider(string scopeName)
        {
            var provider = new SqlSyncProvider();
            provider.ScopeName = scopeName;
            provider.Connection = new SqlConnection(conString);
            provider.ObjectSchema = "dbo";
        
            // create anew scope description and add the appropriate tables to this scope
            var scopeDesc = new DbSyncScopeDescription(scopeName);

            // class to be used to provision the scope defined above
            var serverConfig =
                new SqlSyncScopeProvisioning((SqlConnection) provider.Connection);
            serverConfig.ObjectSchema = "dbo";

            //determine if this scope already exists on the server and if not go ahead 
            //and provision
            if (!serverConfig.ScopeExists(scopeName))
            {
                // note that it is important to call this after the tables have been added 
                // to the scope
                serverConfig.PopulateFromScopeDescription(scopeDesc);

                //indicate that the base table already exists and does not need to be created
                serverConfig.SetCreateTableDefault(DbSyncCreationOption.Skip);

                //provision the server
                serverConfig.Apply();
            }


            return provider;
        }
    }
}