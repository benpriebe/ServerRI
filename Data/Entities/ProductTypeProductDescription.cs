#region Using directives

using System;

#endregion


namespace Data.Entities
{
    public class ProductTypeProductDescription
    {
        public int ProductTypeID { get; set; }
        public int ProductDescriptionID { get; set; }
        public string Culture { get; set; }
        public Guid rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }
        public virtual ProductDescription ProductDescription { get; set; }
        public virtual ProductType ProductType { get; set; }
    }
}