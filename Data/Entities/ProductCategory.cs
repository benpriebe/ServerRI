#region Using directives

using System;
using System.Collections.Generic;

#endregion


namespace Data.Entities
{
    public class ProductCategory
    {
        public ProductCategory()
        {
            Products = new List<Product>();
            ProductCategory1 = new List<ProductCategory>();
        }

        public int ProductCategoryID { get; set; }
        public int? ParentProductCategoryID { get; set; }
        public string Name { get; set; }
        public Guid rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<ProductCategory> ProductCategory1 { get; set; }
        public virtual ProductCategory ProductCategory2 { get; set; }
    }
}