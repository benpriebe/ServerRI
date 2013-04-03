using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Products
{
    public class ProductModelUpdateRequest
    {
        [Display, Range(1, Int32.MaxValue)]
        public int ProductID { get; set; }
        
        [Range(0, 9999), Display]
        public decimal StandardCost { get; set; }

        [Range(0, 99999), Display]
        public decimal ListPrice { get; set; }

        [Display]
        public int ProductCategoryID { get; set; }

        [Display]
        public virtual ProductCategoryModel ProductCategory { get; set; }
    }

    public class ProductCategoryModel
    {
        [Display]
        public int ProductCategoryID { get; set; }
        
        [Display]
        public int? ParentProductCategoryID { get; set; }
        
        [Display]
        public string Name { get; set; }
    }
}