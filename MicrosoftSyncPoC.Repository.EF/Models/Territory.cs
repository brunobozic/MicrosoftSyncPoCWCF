using System;
using System.Collections.Generic;

namespace MicrosoftSyncPoC.Repository.EF.Models
{
    public class Territory
    {
        public Territory()
        {
            Employees = new List<Employee>();
        }

        public Guid TerritoryID { get; set; }
        public string TerritoryDescription { get; set; }
        public Guid? RegionID { get; set; }
        public virtual Region Region { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}