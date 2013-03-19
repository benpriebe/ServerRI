using System.ComponentModel.DataAnnotations;

namespace Models.Administration.Products
{
    public class ProductModelUpdateRequest
    {
        public int ProductID { get; set; }
        
        [Range(0, 9999)]
        public decimal StandardCost { get; set; }

        [Range(0, 99999)]
        public decimal ListPrice { get; set; }
    }
}