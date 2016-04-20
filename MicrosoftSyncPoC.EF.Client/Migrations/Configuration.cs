using System.Data.Entity.Migrations;
using MicrosoftSyncPoC.EF.Client.Models;

namespace MicrosoftSyncPoC.EF.Client.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<northwind_guid_clientContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(northwind_guid_clientContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}