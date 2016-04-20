using System.Collections.ObjectModel;
using System.Data.SqlClient;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServer;

namespace MicrosoftSyncPoC.ServerSide
{
    public class ServerSideSetup
    {
        public static void Deprovision()
        {
            // Connection to  SQL Server database
            var serverConn =
                new SqlConnection(
                    @"Data Source=KINGDEVLAP36\MSSQLSERVER12;Initial Catalog=northwind;Integrated Security=True");

            // Connection to SQL client database
            var clientConn =
                new SqlConnection(
                    @"Data Source=KINGDEVLAP36\MSSQLSERVER12;Initial Catalog=northwind-client;Integrated Security=True");

            // Create Scope Deprovisioning for Sql Server and SQL client.
            var serverSqlDepro = new SqlSyncScopeDeprovisioning(serverConn);
            var clientSqlDepro = new SqlSyncScopeDeprovisioning(clientConn);

            // Remove the scope from SQL Server remove all synchronization objects.
            serverSqlDepro.DeprovisionScope("customers");
            serverSqlDepro.DeprovisionStore();

            // Remove the scope from SQL client and remove all synchronization objects.
            clientSqlDepro.DeprovisionScope("customers");
            clientSqlDepro.DeprovisionStore();

            // Shut down database connections.
            serverConn.Close();
            serverConn.Dispose();
            clientConn.Close();
            clientConn.Dispose();
        }

        public static void SetUp()
        {
            // Connection to on  SQL Server database
            var serverConn =
                new SqlConnection(
                    @"Data Source=KINGDEVLAP36\MSSQLSERVER12;Initial Catalog=northwind;Integrated Security=True");

            // Connection to SQL client database
            var clientConn =
                new SqlConnection(
                    @"Data Source=KINGDEVLAP36\MSSQLSERVER12;Initial Catalog=northwind-client;Integrated Security=True");

            // Create a scope named "customer" and add tables to it.
            var customersScope = new DbSyncScopeDescription("customers");

            // Select the colums to be included in the Collection Object
            var includeColumns = new Collection<string>
            {
                "CustomerID",
                "CompanyName",
                "ContactName",
                "ContactTitle",
                "Address",
                "City",
                "Region",
                "PostalCode",
                "Country",
                "Phone",
                "Fax",
                "Orders",
                "CustomerDemographics"
            };

            // Define the Products table.
            var customerDescription = SqlSyncDescriptionBuilder.GetDescriptionForTable("dbo.Customers", includeColumns,
                serverConn);

            // Add the Table to the scope object.    
            customersScope.Tables.Add(customerDescription);

            // Create a provisioning object for "product" and apply it to the on-premise database if one does not exist.
            var serverProvision = new SqlSyncScopeProvisioning(serverConn, customersScope);


            // Filter Rows for the ListPrice column
            // serverProvision.Tables["dbo.Customer"].AddFilterColumn("ListPrice");
            // serverProvision.Tables["dbo.Customer"].FilterClause = "[side].[ListPrice] < '600'";

            if (!serverProvision.ScopeExists("customers"))
                serverProvision.Apply();

            // Provision the SQL client database from the on-premise SQL Server database if one does not exist.
            var clientProvision = new SqlSyncScopeProvisioning(clientConn, customersScope);


            if (!clientProvision.ScopeExists("customers"))
                clientProvision.Apply();

            // Shut down database connections.
            serverConn.Close();

            serverConn.Dispose();

            clientConn.Close();

            clientConn.Dispose();
        }
    }
}