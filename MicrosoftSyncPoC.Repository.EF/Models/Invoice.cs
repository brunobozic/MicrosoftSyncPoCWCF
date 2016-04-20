using System;
using System.Collections.Generic;

namespace MicrosoftSyncPoC.Repository.EF.Models
{
    public class Invoice
    {
        public Guid InvoiceID { get; set; }
        public Guid ShipperID { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipRegion { get; set; }
        public string ShipPostalCode { get; set; }
        public string ShipCountry { get; set; }
        public Guid CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public Guid RegionID { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public Guid SalespersonID { get; set; }
        public Guid OrderID { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string ShipperName { get; set; }
        public Guid ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }
        public decimal? ExtendedPrice { get; set; }
        public decimal? Freight { get; set; }
        public virtual Shipper Shipper { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Region Region { get; set; }
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
        public virtual Employee Employee { get; set; }
    }
}