#region Using directives

using System;
using System.Collections.Generic;

#endregion


namespace Data.Entities
{
    public class ProductDescription
    {
        public ProductDescription()
        {
            ProductTypeProductDescriptions = new List<ProductTypeProductDescription>();
        }

        public int ProductDescriptionID { get; set; }
        public string Description { get; set; }
        public Guid rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }
        public virtual ICollection<ProductTypeProductDescription> ProductTypeProductDescriptions { get; set; }
    }
}