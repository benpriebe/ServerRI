using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Contracts.Data;
using Models.Resx;

namespace Models.Administration.Products
{
    public class ProductModelCreateRequest : ProductModel, IValidatableObject
    {
        public int? ProductCategoryID { get; set; }
        public int? ProductTypeId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ProductNumber == "1" && StandardCost > 99)
            {
                yield return new ValidationResult(String.Format(LocalizedErrors.ProductModelCreateRequest_ErrorCode_1, StandardCost, ProductNumber), new[]
                {
                    "ProductNumber", "StandardCost"
                });
            }
        }
    }
}