using System;
using System.Collections.Generic;

namespace MicrosoftSyncPoC.EF.Client.Models
{
    public partial class Territory
    {
        public Territory()
        {
            this.Employees = new List<Employee>();
        }

        public System.Guid TerritoryID { get; set; }
        public string TerritoryDescription { get; set; }
        public System.Guid RegionID { get; set; }
        public virtual Region Region { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
