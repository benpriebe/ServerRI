using System.ComponentModel.DataAnnotations;
using Models.Administration;

namespace Models.Products
{
    public class ProductModelFilterRequest : FilterRequest
    {
        [Display]
        public string ProductName { get; set; }
    }
}