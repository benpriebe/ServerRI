using System.ComponentModel.DataAnnotations;

namespace Models.Products
{
    public class ProductModelUpdateRequestBase : ModelBase
    {
        private int _productCategoryId;

        [Display]
        public int ProductCategoryID
        {
            get { return _productCategoryId; }
            set
            {
                _productCategoryId = value;
                OnPropertyChanged();
            }
        }
    }
}