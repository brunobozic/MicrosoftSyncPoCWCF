using System;
using System.Collections.Generic;

namespace MicrosoftSyncPoC.Repository.EF.Models
{
    public class Region
    {
        public Region()
        {
            Territories = new List<Territory>();
        }

        public Guid RegionID { get; set; }
        public string RegionDescription { get; set; }
        public virtual ICollection<Territory> Territories { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
    }
}