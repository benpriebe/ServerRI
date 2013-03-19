using System.ComponentModel.DataAnnotations;
using Models.Resx;

namespace Models.Administration.Products
{
    public class ProductModel 
    {
        public int Id { get; internal set; }

        [Required(ErrorMessageResourceType = typeof(LocalizedErrors), ErrorMessageResourceName = "Required")]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductNumber { get; set; }

        [Range(0, 9999)]
        public decimal StandardCost { get; set; }

        [Range(0, 99999)]
        public decimal ListPrice { get; set; }

    }
}