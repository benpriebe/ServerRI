#region Using directives

using System.ComponentModel.DataAnnotations;

#endregion


namespace Models.Products
{
    public class ProductModel 
    {
        [Display]
        public int Id { get; internal set; }

        [Required, StringLength(50), Display]
        public string Name { get; set; }

        [Required, StringLength(50), Display]
        public string ProductNumber { get; set; }

        [Range(0, 9999), Display]
        public decimal StandardCost { get; set; }

        [Range(0, 9999), Display]
        public decimal ListPrice { get; set; }
    }
}