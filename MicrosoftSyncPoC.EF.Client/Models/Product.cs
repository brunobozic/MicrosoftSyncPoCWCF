using System;
using System.Collections.Generic;

namespace MicrosoftSyncPoC.EF.Client.Models
{
    public partial class Product
    {
        public Guid ProductID { get; set; }
        public string ProductName { get; set; }
        public Nullable<Guid> SupplierID { get; set; }
        public Nullable<Guid> CategoryID { get; set; }
        public string QuantityPerUnit { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<short> UnitsInStock { get; set; }
        public Nullable<short> UnitsOnOrder { get; set; }
        public Nullable<short> ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
        public virtual Category Category { get; set; }
        public virtual Supplier Supplier { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
    }
}
