using System;
using System.Collections.Generic;

namespace MicrosoftSyncPoC.EF.Client.Models
{
    public partial class Order
    {
        public Guid OrderID { get; set; }
        public Nullable<Guid> CustomerID { get; set; }
        public Nullable<Guid> EmployeeID { get; set; }
        public Nullable<DateTime> OrderDate { get; set; }
        public Nullable<DateTime> RequiredDate { get; set; }
        public Nullable<DateTime> ShippedDate { get; set; }
        public Nullable<Guid> ShipVia { get; set; }
        public Nullable<decimal> Freight { get; set; }
        public string ShipName { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipRegion { get; set; }
        public string ShipPostalCode { get; set; }
        public string ShipCountry { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Shipper Shipper { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
    }
}
