using System;
using System.Collections.Generic;

namespace MicrosoftSyncPoC.EF.Client.Models
{
    public partial class Region
    {
        public Region()
        {
            this.Territories = new List<Territory>();
        }

        public Guid RegionID { get; set; }
        public string RegionDescription { get; set; }
        public virtual ICollection<Territory> Territories { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
    }
}
