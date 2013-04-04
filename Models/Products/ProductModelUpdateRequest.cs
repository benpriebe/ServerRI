#region Using directives

using System;
using System.ComponentModel.DataAnnotations;

#endregion


namespace Models.Products
{
    public class ProductModelUpdateRequest : ProductModelUpdateRequestBase
    {
        private int _productId;
        private decimal _listPrice;
        private decimal _standardCost;
        private ProductCategoryModel _productCategory;


        [Display, Range(1, Int32.MaxValue)]
        public int ProductID
        {
            get { return _productId; }
            set
            {
                _productId = value;
                OnPropertyChanged();
            }
        }

        [Range(0, 9999), Display]
        public decimal StandardCost
        {
            get { return _standardCost; }
            set
            {
                _standardCost = value;
                OnPropertyChanged();
            }
        }

        [Range(0, 99999), Display]
        public decimal ListPrice
        {
            get { return _listPrice; }
            set
            {
                _listPrice = value;
                OnPropertyChanged();
            }
        }

        [Display]
        public virtual ProductCategoryModel ProductCategory
        {
            get { return _productCategory; }
            set
            {
                _productCategory = value;
                OnPropertyChanged();
            }
        }
    }

    public class ProductCategoryModel : ModelBase
    {
        private string _name;

        [Display]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
    }
}