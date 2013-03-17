namespace Models.Administration.Products
{
    public class ProductModelCreateRequest : ProductModel
    {
        public int? ProductCategoryID { get; set; }
        public int? ProductTypeId { get; set; }
    }
}