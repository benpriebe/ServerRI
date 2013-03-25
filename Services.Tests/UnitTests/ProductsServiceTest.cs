#region Using directives

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Api.Common;
using Autofac;
using Contracts.Data;
using Data.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Products;
using Moq;
using Providers;
using Services.Resx;

#endregion

namespace Services.Tests.UnitTests
{
    [TestClass]
    public class ProductsServiceTest
    {
        private ProductsService _service;
        private Mock<IProductsProvider> _mockProvider;

        [TestInitialize]
        public void Initialize()
        {
            //TODO: 15-Mar-2013 - Ben - seems hacky rebuilding container. I should just be able to override the mock provider instance each time. Seems to only remember the first time.
            // to replicate remove the IoC.BuildContainer() line and run all tests. The mock instance is only registered once for some reason.
            // come back to this one when you know a bit more.
            IoC.BuildContainer();

            var builder = new ContainerBuilder();
            _mockProvider = new Mock<IProductsProvider>();

            builder.RegisterInstance(new Mock<DbContext>().Object);
            builder.RegisterInstance(_mockProvider.Object);
            builder.Update(IoC.Container);

            var scope = IoC.Container.BeginLifetimeScope();
            _service = scope.Resolve<ProductsService>();
        }

        [TestMethod]
        public void AddProduct_BareBones()
        {
            //Arrange 
            var productModel = new ProductModelCreateRequest
            {
                Name = "iPhone 7",
                ListPrice = 999.90M,
                ProductNumber = "A62037",
                StandardCost = 324.23M
            };

            const int productId = 99;
            _mockProvider.Setup(p => p.Add(It.IsAny<Product>())).Callback<Product>((p) =>
            {
                p.ProductID = productId;
            });

            //Act
            var result = _service.AddProduct(productModel);

            //Assert
            _mockProvider.VerifyAll();
            Assert.IsTrue(result.Success);
            Assert.IsTrue(productModel.Id == productId);
        }

        [TestMethod]
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

            //Act
            var result = _service.AddProduct(productModel);

            //Assert
            Assert.IsTrue(result.Failure);
            Assert.IsTrue(result.Messages.Any());
            Assert.IsTrue(result.Messages.Any(m => m.Code == (int)MessageCodes.ValidationError));
            Assert.IsTrue(result.Messages.Any(m => String.Format(Models.Products.Resx.Strings.ProductModelCreateRequest_ValidationError_1, productModel.StandardCost, productModel.ProductNumber) == m.Phrase));
        }

        [TestMethod]
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

            _mockProvider.Setup(p => p.GetAll()).Returns(new []{ new Product { ProductNumber ="HL-U509" }}.AsQueryable());
            
            //Act
            var result = _service.AddProduct(productModel);

            //Assert
            Assert.IsTrue(result.Failure);
            Assert.IsTrue(result.Messages.Any());
            Assert.IsTrue(result.Messages.Any(m => m.Code == (int)MessageCodes.ValidationError));
            Assert.IsTrue(result.Messages.Any(m => String.Format(Strings.ProductsService_AddProduct_ValidationError_1, productModel.ProductNumber) == m.Phrase));
        }


        [TestMethod]
        public void AddProduct_ViolatesSqlConstraints()
        {
            var productModel = new ProductModelCreateRequest
            {
                Name = "iPhone 7",
                ListPrice = 999.90M,
                ProductNumber = "A62037",
                StandardCost = 324.23M
            };

            _mockProvider.Setup(p => p.Add(It.IsAny<Product>())).Throws(new ProviderException("Provider Exception", new[]
            {
                "Error 1", "Error 2"
            }));

            //Act
            var result = _service.AddProduct(productModel);

            _mockProvider.VerifyAll();
        }

        [TestMethod]
        public void UpdateProduct_Success()
        {
            throw new NotImplementedException("Left as an excercise for the reader");
        }

        [TestMethod]
        public void UpdateProduct_Failure()
        {
            throw new NotImplementedException("Left as an excercise for the reader");
        }

        [TestMethod]
        public void GetProductById_InvalidProductID()
        {
            //Arrange
            const int productId = -99;
            _mockProvider.Setup(p => p.GetById(productId)).Returns((Product) null);

            //Act
            var result = _service.GetProductById(productId);

            //Assert
            _mockProvider.VerifyAll();
            Assert.IsTrue(result.NotFound);
            Assert.IsTrue(result.Failure);
            Assert.IsTrue(result.Messages.Any(m => m.Code == (int)MessageCodes.NotFound));

        }

        [TestMethod]
        public void GetProductById_ValidProductID()
        {
            //Arrange 
            const int productId = 1;
            var product = GetTestProducts(1).Single();

            _mockProvider.Setup(p => p.GetById(productId)).Returns(product);

            //Act
            var result = _service.GetProductById(productId);

            //Assert
            _mockProvider.VerifyAll();
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Value);

            Assert.AreEqual(product.Name, result.Value.Name);

            // Category information
            Assert.AreEqual(product.ProductCategoryID, result.Value.ProductCategoryID);
            Assert.AreEqual(product.ProductCategory.Name, result.Value.CategoryName);
            Assert.AreEqual(product.ProductCategory.ParentProductCategoryID, result.Value.ParentProductCategoryID);
        }

        [TestMethod]
        public void GetProducts_Filtered()
        {
            throw new NotImplementedException("Left as an excercise for the reader.");
        }

        [TestMethod]
        public void GetProducts_WithResults()
        {
            //Arrange
            var products = new List<Product>(GetTestProducts(5));

            _mockProvider.Setup(p => p.GetAll()).Returns(products.AsQueryable());

            //Act
            var result = _service.GetProducts();

            //Assert
            _mockProvider.VerifyAll();
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(5, result.Value.Count);
        }

        [TestMethod]
        public void GetProductsWithDetails_WithResults()
        {
            //Arrange
            var products = new List<Product>(GetTestProducts(5));

            _mockProvider.Setup(p => p.GetProductsWithDetails()).Returns(products.AsQueryable());

            //Act
            var result = _service.GetProductsWithDetails();

            //Assert
            _mockProvider.VerifyAll();
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(5, result.Value.Count);

            Assert.IsNotNull(result.Value[0].ProductCategoryID);
            // etc.
        }


        [TestMethod]
        public void DeleteProduct_InvalidProductId()
        {
            //Arrange
            const int productId = -99;
            _mockProvider.Setup(p => p.Delete(productId));

            //Act
            var result = _service.DeleteProduct(productId);

            //Assert
            _mockProvider.VerifyAll();
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public void DeleteProduct_ValidProductId()
        {
            //Arrange
            const int productId = 1;
            _mockProvider.Setup(p => p.Delete(productId));

            //Act
            var result = _service.DeleteProduct(productId);

            //Assert
            _mockProvider.VerifyAll();
            Assert.IsTrue(result.Success);
        }

        private static IEnumerable<Product> GetTestProducts(int howMany)
        {
            for (int i = 0; i < howMany; i++)
            {
                yield return new Product
                {
                    Name = "Product " + i.ToString(),
                    ListPrice = 999.90M,
                    ProductNumber = "A62037",
                    StandardCost = 324.23M,
                    ProductCategoryID = 1,
                    ProductCategory = new ProductCategory
                    {
                        ProductCategoryID = 1,
                        Name = "Road Frames",
                        ParentProductCategoryID = 2
                    }
                };
            }
        }
    }
}
