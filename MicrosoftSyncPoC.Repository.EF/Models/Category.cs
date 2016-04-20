using System;
using System.Collections.Generic;

namespace MicrosoftSyncPoC.Repository.EF.Models
{
    public class Category
    {
        public Category()
        {
            Products = new List<Product>();
        }

        public Guid CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}