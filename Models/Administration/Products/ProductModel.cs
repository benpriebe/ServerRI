namespace Models.Administration.Products
{
    public class ProductModel
    {
        public int Id { get; internal set; }
        public string Name { get; set; }
        public string ProductNumber { get; set; }
        public decimal StandardCost { get; set; }
        public decimal ListPrice { get; set; }
    }
}