#region Using directives

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Api.Client;
using Api.Common;
using Autofac.Integration.WebApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Products;
using Models.Products.Resx;
using WebApi.Controllers;

#endregion


namespace WebApi.Tests.Controllers
{
    [TestClass]
    public class ProductsControllerTest 
    {
        public const string BaseUri = "http://localhost:60000/api/products/";
        
        private readonly ClientProxy _productClient = new ClientProxy(BaseUri, (request) => request.Headers.Add("TransactionRollback", "true"));

        [TestMethod]
        public void Verify_Routing_Rules()
        {
            // Initialize AutoFac WebApi stuff 
            IoC.BuildContainer();
            var config = new HttpConfiguration();
            var webApiDependencyResolver = new AutofacWebApiDependencyResolver(IoC.Container);
            config.DependencyResolver = webApiDependencyResolver;
            WebApiConfig.RegisterRoutes(config.Routes);
            

            // arrange/act/assert
            var route = RouteInfo.Verify(config, HttpMethod.Get, BaseUri + "1", typeof(ProductsController), "Default");
            route.VerifyRouteData("id", "1");

            // arrange/act/assert
            route = RouteInfo.Verify(config, HttpMethod.Get, BaseUri, typeof(ProductsController), "Default");

            // arrange/act/assert
            route = RouteInfo.Verify(config, HttpMethod.Get, BaseUri + "details", typeof(ProductsController), "Details");
            
            // arrange/act/assert
            route = RouteInfo.Verify(config, HttpMethod.Get, BaseUri + "?productname=blah&top=1&skip=10", typeof(ProductsController), "Default");
            Assert.IsTrue(route.Params.Any(p => p.ParameterType == typeof(ProductModelFilterRequest)));
            
            // arrange/act/assert
            route = RouteInfo.Verify(config, HttpMethod.Post, BaseUri, typeof(ProductsController), "Default");

            // arrange/act/assert
            route = RouteInfo.Verify(config, HttpMethod.Put, BaseUri + "1", typeof(ProductsController), "Default");

            // arrange/act/assert
            route = RouteInfo.Verify(config, HttpMethod.Put, BaseUri + "1/mark-sold-out", typeof(ProductsController), "MarkSoldOut");

            // arrange/act/assert
            route = RouteInfo.Verify(config, HttpMethod.Delete, BaseUri + "1", typeof(ProductsController), "Default");
        }
        
        [TestMethod]
        [TestCategory("Database")]
        public async Task AddProduct_BareBones()
        {
            //Arrange
            var product = new ProductModelCreateRequest
            {
                Name = "iPhone 7",
                ListPrice = 999.90M,
                ProductNumber = "A62037",
                StandardCost = 324.23M
            };

            //Act
            var added = await _productClient.PostWithResult<ProductModelCreateRequest, string>(product);

            //Assert
            Assert.IsTrue(added.Success);
            Assert.IsNotNull(added.Value);
        }

        [TestMethod]
        [TestCategory("Database")]
        public async Task AddProduct_ModelValidationFails()
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
            var result = await _productClient.PostWithUri(productModel);

            //Assert
            Assert.IsTrue(result.Failure);
            Assert.IsTrue(result.Messages.Any());
            Assert.IsTrue(result.Messages.Any(m => m.Code == (int) MessageCodes.ValidationError));
            Assert.IsTrue(result.Messages.Any(m => String.Format(Strings.ProductModelCreateRequest_ValidationError_1, productModel.StandardCost, productModel.ProductNumber) == m.Phrase));
        }

        [TestMethod]
        [TestCategory("Database")]
        public async Task AddProduct_ValidationFailsDatabaseLookup()
        {
            //Arrange 
            var productModel = new ProductModelCreateRequest
            {
                Name = "iPhone 7",
                ListPrice = 999.90M,
                ProductNumber = "HL-U509",
                StandardCost = 324.23M
            };

            //Act
            var result = await _productClient.PostWithUri(productModel);

            //Assert
            Assert.IsTrue(result.Failure);
            Assert.IsTrue(result.Messages.Any());
            Assert.IsTrue(result.Messages.Any(m => m.Code == (int) MessageCodes.ValidationError));
            Assert.IsTrue(result.Messages.Any(m => String.Format(Services.Resx.Strings.ProductsService_AddProduct_ValidationError_1, productModel.ProductNumber) == m.Phrase));
        }

        [TestMethod]
        [TestCategory("Database")]
        public async Task UpdateProduct_Success()
        {
            throw new NotImplementedException("Left as an excercise for the reader");
        }

        [TestMethod]
        [TestCategory("Database")]
        public async Task UpdateProduct_Failure()
        {
            throw new NotImplementedException("Left as an excercise for the reader");
        }

        [TestMethod]
        [TestCategory("Database")]
        public async Task GetProductById_ValidProductID()
        {
            //Act
            var result = await _productClient.Get<ProductModelResponse>(680);

            //Assert
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Value);

            Assert.AreEqual("HL Road Frame - Black, 58", result.Value.Name);
            // etc.
        }

        [TestMethod]
        [TestCategory("Database")]
        public async Task GetProductById_InvalidProductID()
        {
            //Act
            var result = await _productClient.Get<ProductModelResponse>(1);

            //Assert
            Assert.IsTrue(result.NotFound);
            Assert.IsTrue(result.Failure);
            Assert.IsTrue(result.Messages.Any(m => m.Code == (int) MessageCodes.NotFound));
        }

        [TestMethod]
        [TestCategory("Database")]
        public async Task GetProducts_Filtered()
        {
            throw new NotImplementedException("Left as an excercise for the reader.");
        }

        [TestMethod]
        [TestCategory("Database")]
        public async Task GetProducts_WithResults()
        {
            //Act
            var result = await _productClient.GetList<ProductModelResponse>();

            //Assert
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Value);
            Assert.IsTrue(result.Value.Any());
        }

        [TestMethod]
        [TestCategory("Database")]
        public async Task GetProductsWithDetails_WithResults()
        {
            //Act
            var result = await _productClient.GetList<ProductModelResponse>("details");

            //Assert
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Value);
            Assert.IsTrue(result.Value.Any());

            Assert.IsNotNull(result.Value[0].ProductCategoryID);
            // etc.
        }


        [TestMethod]
        [TestCategory("Database")]
        public async Task DeleteProduct_ValidProductId()
        {
            //Arrange
            //Act
            var result = await _productClient.Delete(680);

            //Assert
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        [TestCategory("Database")]
        public async Task DeleteProduct_InvalidProductId()
        {
            //Arrange
            const int productId = -9876;
            // Act
            var result = await _productClient.Delete(productId);

            // Assert
            Assert.IsTrue(result.NotFound);
            Assert.IsTrue(result.Failure);
            Assert.IsTrue(result.Messages.Any(m => m.Code == (int) MessageCodes.NotFound));
        }
    }
}