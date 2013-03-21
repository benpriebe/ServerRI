using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Api.Client;
using Api.Common;
using Api.Common.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Administration.Products;


//TODO: 20-Mar-2013 - Ben - Work in Progress. Not for consumption.
namespace WebApi.Tests.Controllers
{
    public class TestProxy : ClientProxy
    {
        public TestProxy(Link link) : base(link)
        {
        }

        public TestProxy(Endpoint endpoint, string linkId, params string[] methods) : base(endpoint, linkId, methods)
        {
        }

        public Result<List<ProductModelResponse>> GetList()
        {
            return GetList<ProductModelResponse>();
        }

    }

    [TestClass]
    public class ProductsControllerTest
    {
        [TestMethod]
        public void TestMethod1()
        {

            var ep = new Endpoint("id", Assembly.GetExecutingAssembly(), Assembly.GetExecutingAssembly());
            ep.Add("products", "/api/products", "GET");
            ep.SetBaseUri(new Uri("http://localhost:60000/"));

            var proxy = new TestProxy(ep, "products", "GET");

            var x = proxy.GetList();

            Assert.IsTrue(x.Success);
            Assert.IsTrue(x.Value.Any());
        }

        [TestMethod]
        public void TestMethod2()
        {


            var proxy = new Proxy(new Uri("http://localhost:60000/"));

            var x = proxy.Get<IList<ProductModel>>(new Uri("http://localhost:60000/api/products/"));

            Assert.IsTrue(x.Success);
            Assert.IsTrue(x.Value.Any());
        }
    }
}
