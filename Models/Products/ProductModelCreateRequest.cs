using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Models.Products.Resx;

namespace Models.Products
{
    public class ProductModelCreateRequest : ProductModel, IValidatableObject
    {
        [Display]
        public int? ProductCategoryID { get; set; }
        
        [Display]
        public int? ProductTypeId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ProductNumber == "1" && StandardCost > 99)
            {
                yield return new ValidationResult(String.Format(Strings.ProductModelCreateRequest_ValidationError_1, StandardCost, ProductNumber), new[]
                {
                    "ProductNumber", "StandardCost" // todo: replace these with localizable values.
                });
            }
        }
    }
}