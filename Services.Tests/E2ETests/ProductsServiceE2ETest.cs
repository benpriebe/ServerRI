#region Using directives

using System;
using System.Linq;
using System.Transactions;
using Api.Common;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Administration.Products;
using Models.Resx;

#endregion


namespace Services.Tests.E2ETests
{
    [TestClass]
    public class ProductsServiceE2ETest
    {
        protected ProductsService _service = IoC.Container.Resolve<ProductsService>();

        [TestMethod]
        [TestCategory("Database")]
        public void AddProduct_BareBones()
        {
            //Arrange
            var product = new ProductModelCreateRequest
            {
                Name = "iPhone 7",
                ListPrice = 999.90M,
                ProductNumber = "A62037",
                StandardCost = 324.23M
            };

            using (new TransactionScope())
            {
                //Act
                _service.AddProduct(product);

                //Assert
                var result = _service.GetProductById(product.Id);

                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.Value);

                Assert.AreEqual(result.Value.Name, product.Name);
                Assert.AreEqual(result.Value.ListPrice, product.ListPrice);
                Assert.AreEqual(result.Value.StandardCost, product.StandardCost);
                Assert.IsTrue(result.Value.LastModifiedDate < DateTime.Now && result.Value.LastModifiedDate > DateTime.MinValue);
            }
        }

        [TestMethod]
        [TestCategory("Database")]
        public void AddProduct_ModelValidationFails()
        {
            //Arrange 
            var productModel = new ProductModelCreateRequest
            {
                Name = "iPhone 7",
                ListPrice = 999.90M,
                ProductNumber = "1",
                StandardCost = 324.23M
            };

            using (new TransactionScope())
            {
                //Act
                var result = _service.AddProduct(productModel);

                //Assert
                Assert.IsTrue(result.Failure);
                Assert.IsTrue(result.Messages.Any());
                Assert.IsTrue(result.Messages.Any(m => m.Code == (int) MessageCodes.ValidationError));
                Assert.IsTrue(result.Messages.Any(m => String.Format(LocalizedErrors.ProductModelCreateRequest_ErrorCode_1, productModel.StandardCost, productModel.ProductNumber) == m.Phrase));
            }
        }

        [TestMethod]
        [TestCategory("Database")]
        public void AddProduct_ValidationFailsDatabaseLookup()
        {
            //Arrange 
            var productModel = new ProductModelCreateRequest
            {
                Name = "iPhone 7",
                ListPrice = 999.90M,
                ProductNumber = "HL-U509",
                StandardCost = 324.23M
            };

            using (new TransactionScope())
            {
                //Act
                var result = _service.AddProduct(productModel);

                //Assert
                Assert.IsTrue(result.Failure);
                Assert.IsTrue(result.Messages.Any());
                Assert.IsTrue(result.Messages.Any(m => m.Code == (int) MessageCodes.ValidationError));
                Assert.IsTrue(result.Messages.Any(m => String.Format(LocalizedErrors.ProductsService_AddProduct_ErrorCode1, productModel.ProductNumber) == m.Phrase));
            }
        }

        [TestMethod]
        [TestCategory("Database")]
        public void UpdateProduct_Success()
        {
            throw new NotImplementedException("Left as an excercise for the reader");
        }

        [TestMethod]
        [TestCategory("Database")]
        public void UpdateProduct_Failure()
        {
            throw new NotImplementedException("Left as an excercise for the reader");
        }

        [TestMethod]
        [TestCategory("Database")]
        public void GetProductById_ValidProductID()
        {
            //Act
            var result = _service.GetProductById(680);

            //Assert
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Value);

            Assert.AreEqual("HL Road Frame - Black, 58", result.Value.Name);
            // etc.
        }

        [TestMethod]
        [TestCategory("Database")]
        public void GetProductById_InvalidProductID()
        {
            //Act
            var result = _service.GetProductById(1);
            
            //Assert
            Assert.IsTrue(result.NotFound);
            Assert.IsTrue(result.Failure);
            Assert.IsTrue(result.Messages.Any(m => m.Code == (int)MessageCodes.NotFound));

        }

        [TestMethod]
        [TestCategory("Database")]
        public void GetProducts_Filtered()
        {
            throw new NotImplementedException("Left as an excercise for the reader.");
        }

        [TestMethod]
        [TestCategory("Database")]
        public void GetProducts_WithResults()
        {
            //Act
            var result = _service.GetProducts();

            //Assert
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Value);
            Assert.IsTrue(result.Value.Any());
        }

        [TestMethod]
        [TestCategory("Database")]
        public void GetProductsWithDetails_WithResults()
        {
            //Act
            var result = _service.GetProductsWithDetails();

            //Assert
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Value);
            Assert.IsTrue(result.Value.Any());

            Assert.IsNotNull(result.Value[0].ProductCategoryID);
            // etc.
        }


        [TestMethod]
        [TestCategory("Database")]
        public void DeleteProduct_ValidProductId()
        {
            //Arrange
            var product = new ProductModelCreateRequest
            {
                Name = "iPhone 7",
                ListPrice = 999.90M,
                ProductNumber = "A62037",
                StandardCost = 324.23M
            };

            using (new TransactionScope())
            {
                //Act
                _service.AddProduct(product);

                //Assert
                var deleteResult = _service.DeleteProduct(product.Id);
                Assert.IsTrue(deleteResult.Success);

                var result = _service.GetProductById(product.Id);
                Assert.IsTrue(result.NotFound);
                Assert.IsTrue(result.Failure);
                Assert.IsTrue(result.Messages.Any(m => m.Code == (int)MessageCodes.NotFound));

            }
        }

        [TestMethod]
        [TestCategory("Database")]
        public void DeleteProduct_InvalidProductId()
        {
            //Arrange
            const int productId = -9876;
            using (new TransactionScope())
            {
                // Act
                var result = _service.DeleteProduct(productId);

                // Assert
                Assert.IsTrue(result.NotFound);
                Assert.IsTrue(result.Failure);
                Assert.IsTrue(result.Messages.Any(m => m.Code == (int)MessageCodes.NotFound));
            }
        }
    }
}