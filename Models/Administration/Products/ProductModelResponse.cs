#region Using directives

using System;

#endregion


namespace Models.Administration.Products
{
    public class ProductModelResponse : ProductModel
    {
        public DateTime LastModifiedDate { get; internal set; }

        public int? ProductCategoryID { get; set; }
        public int? ParentProductCategoryID { get; set; }
        public string CategoryName { get; set; }

        public int? ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }
        public string CatalogDescription { get; set; }
    }
}