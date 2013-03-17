#region Using directives

using System;
using System.Linq;
using System.Transactions;
using Autofac;
using Contracts.Data;
using Data.Entities;
using MSTestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion


namespace Providers.Tests.UnitTests.Administration
{
    [TestClass]
    public class ProductsProviderTest
    {
        [TestMethod]
        public void AddProduct_BareBones()
        {
            using (new TransactionScope())
            {
                //Arrange
                var product = GetTestProduct();

                //Act
                using (var uow = IoC.Container.Resolve<IUnitOfWork>())
                {
                    uow.Products.Add(product);
                    uow.Commit();
                }

                //Assert
                using (var uow = IoC.Container.Resolve<IUnitOfWork>())
                {
                    var createdProduct = uow.Products.GetById(product.ProductID);
                    Assert.IsNotNull(createdProduct);
                    Assert.AreEqual(product.Name, createdProduct.Name);
                    //etc..
                }
            }
        }

        [TestMethod]
        public void AddProduct_ViolatesSqlConstraints()
        {
            using (new TransactionScope())
            {
                //Arrage
                var product = GetTestProduct();
                product.ProductCategoryID = null;
                product.ProductCategory = new ProductCategory
                {
                    ProductCategoryID = 1,
                    Name = "Road Frames",
                    ParentProductCategoryID = 2,
                    ModifiedDate = DateTime.UtcNow
                };

                //Act
                using (var uow = IoC.Container.Resolve<IUnitOfWork>())
                {
                    ExceptionAssert.Throws<ProviderException>(() =>
                    {
                        uow.Products.Add(product);
                        uow.Commit();
                    });
                }
            }
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
        public void GetProductById_ValidProductID()
        {
            using (var uow = IoC.Container.Resolve<IUnitOfWork>())
            {
                //Act
                var product = uow.Products.GetById(680);
                //Assert
                Assert.AreEqual("HL Road Frame - Black, 58", product.Name);
                // etc.
            }
        }

        [TestMethod]
        public void GetProductDetailsById_ValidProductID()
        {
            using (var uow = IoC.Container.Resolve<IUnitOfWork>())
            {
                //Act
                var product = uow.Products.GetProductsWithDetails().Single(p => p.ProductID == 680);
                //Assert
                Assert.AreEqual("HL Road Frame - Black, 58", product.Name);

                // Category information
                Assert.AreEqual(18, product.ProductCategoryID);
                Assert.AreEqual("Road Frames", product.ProductCategory.Name);
                Assert.AreEqual(2, product.ProductCategory.ParentProductCategoryID);
                // etc.
            }
        }

        [TestMethod]
        public void GetProductById_InvalidProductID()
        {
            using (var uow = IoC.Container.Resolve<IUnitOfWork>())
            {
                //Act/Assert
                Assert.IsNull(uow.Products.GetById(-99));
            }
        }

        [TestMethod]
        public void GetProducts_Filtered()
        {
            throw new NotImplementedException("Left as an excercise for the reader");
        }

        [TestMethod]
        public void GetAll_WithResults()
        {
            using (var uow = IoC.Container.Resolve<IUnitOfWork>())
            {
                //Act
                var products = uow.Products.GetAll();

                //Assert
                Assert.IsTrue(products.Any());
            }
        }

        [TestMethod]
        public void GetProductsWithDetails_WithResults()
        {
            using (var uow = IoC.Container.Resolve<IUnitOfWork>())
            {
                //Act
                var products = uow.Products.GetProductsWithDetails();

                //Assert
                Assert.IsTrue(products.Any());
            }
        }

        [TestMethod]
        public void DeleteProduct_ValidProductId()
        {
            using (new TransactionScope())
            {
                //Arrange
                var product = GetTestProduct();

                //Act
                using (var uow = IoC.Container.Resolve<IUnitOfWork>())
                {
                    uow.Products.Add(product);
                    uow.Commit();
                }

                //Assert
                using (var uow = IoC.Container.Resolve<IUnitOfWork>())
                {
                    Assert.IsNotNull(uow.Products.GetById(product.ProductID));
                }

                using (var uow = IoC.Container.Resolve<IUnitOfWork>())
                {
                    uow.Products.Delete(product.ProductID);
                    uow.Commit();
                }

                using (var uow = IoC.Container.Resolve<IUnitOfWork>())
                {
                    Assert.IsNull(uow.Products.GetById(product.ProductID));
                }
            }
        }

        [TestMethod]
        public void DeleteProduct_InvalidProductId()
        {
            using (var uow = IoC.Container.Resolve<IUnitOfWork>())
            {
                // Act/Assert
                ExceptionAssert.Throws<ProviderException>(() => uow.Products.Delete(-99));
            }

            using (var uow = IoC.Container.Resolve<IUnitOfWork>())
            {
                Assert.IsNull(uow.Products.GetById(-99));
            }
        }

        public Product GetTestProduct()
        {
            var now = DateTime.UtcNow;
            var product = new Product
            {
                Name = "iPhone 7",
                ListPrice = 999.90M,
                ProductNumber = "A62037",
                StandardCost = 324.23M,
                ModifiedDate = now,
                SellStartDate = now,
                ProductCategoryID = 18,
            };

            return product;
        }
    }
}
       