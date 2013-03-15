namespace Models.Administration.Products
{
    public class ProductModelRequest : ProductModel
    {
        public int? ProductCategoryID { get; set; }
        public int? ProductTypeId { get; set; }
    }
}