#region Using directives

using System;
using System.Collections.Generic;

#endregion


namespace Data.Entities
{
    public class ProductType
    {
        public ProductType()
        {
            Products = new List<Product>();
            ProductTypeProductDescriptions = new List<ProductTypeProductDescription>();
        }

        public int ProductTypeID { get; set; }
        public string Name { get; set; }
        public string CatalogDescription { get; set; }
        public Guid rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<ProductTypeProductDescription> ProductTypeProductDescriptions { get; set; }
    }
}