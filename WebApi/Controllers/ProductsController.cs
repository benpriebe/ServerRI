#region Using directives

using System;
using System.Net.Http;
using System.Web.Http;
using Api.Web.Extensions;
using Autofac.Integration.WebApi;
using Common.Logging;
using Core;
using Models.Products;
using Services;

#endregion

namespace WebApi.Controllers
{
    [AutofacControllerConfiguration]
    public class ProductsController : BaseApiController
    {
        private readonly ProductsService _service;

        public ProductsController(OperationContext context, ILog log, ProductsService service) : base(context, log)
        {
            _service = service;
        }

        // GET {base}/api/products/{id}
        [ActionName("Default")]
        public HttpResponseMessage Get(int id)
        {
            var result = _service.GetProductById(id);
            return Request.CreateGetResponseFor(result);
        }

        // GET {base}/api/products?productname={name}&top={top}&skip={skip}
        [ActionName("Default")]
        public HttpResponseMessage Get([FromUri] ProductModelFilterRequest filter)
        {
            if (filter.ProductName == "error") // note: do not do this in production env.
                throw new Exception("This is to show how exceptions are handled by default in WebAPI");

            var result = _service.GetProducts(filter);
            return Request.CreateGetResponseFor(result);
        }

    }
}