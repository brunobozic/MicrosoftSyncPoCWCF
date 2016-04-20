using System;
using System.Collections.Generic;

namespace MicrosoftSyncPoC.Repository.EF.Models
{
    public class Shipper
    {
        public Shipper()
        {
            Orders = new List<Order>();
        }

        public Guid ShipperID { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}