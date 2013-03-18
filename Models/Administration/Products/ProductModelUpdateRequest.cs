namespace Models.Administration.Products
{
    public class ProductModelUpdateRequest
    {
        public int ProductID { get; set; }
        public decimal StandardCost { get; set; }
        public decimal ListPrice { get; set; }
    }
}