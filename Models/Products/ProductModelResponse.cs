#region Using directives

using System;
using System.ComponentModel.DataAnnotations;

#endregion


namespace Models.Products
{
    public class ProductModelResponse : ProductModel
    {
        [Display]
        public DateTime LastModifiedDate { get; internal set; }

        [Display]
        public int? ProductCategoryID { get; set; }
        
        [Display]
        public int? ParentProductCategoryID { get; set; }
        
        [Display]
        public string CategoryName { get; set; }

        [Display]
        public int? ProductTypeId { get; set; }
        
        [Display]
        public string ProductTypeName { get; set; }
        
        [Display]
        public string CatalogDescription { get; set; }
    }
}