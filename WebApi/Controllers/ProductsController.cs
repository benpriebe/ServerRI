#region Using directives

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Api.Common;
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

        // GET {base}/api/products/?$filter=...insert odata query... 
        // OData Query Syntax -  http://msdn.microsoft.com/en-us/library/ff478141.aspx, http://www.odata.org/documentation/odata-v2-documentation/uri-conventions/
        // NOTE: you can limit the query constraints by adding constraints to this attribute. http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/supporting-odata-query-options
        [Queryable]
        [ActionName("Default")]
        public HttpResponseMessage Get()
        {
            var result = _service.QueryProducts();
            return Request.CreateGetResponseFor(result);
        }

        // GET {base}/api/products/details
        [ActionName("Details")]
        public HttpResponseMessage GetProductWithDetails()
        {
            var result = _service.GetProductsWithDetails();
            return Request.CreateGetResponseFor(result);
        }

        // POST {base}/api/products
        [ActionName("Default")]
        public HttpResponseMessage Post(ProductModelCreateRequest product)
        {
            return Invoke(Request, () =>
            {
                var result = _service.AddProduct(product);
                var response = Request.CreatePostResponseFor(result, WebApiConfig.ProductsRouteName, new
                {
                    id = result.Value != null ? result.Value.ToString() : string.Empty
                });
                return response;
            });
        }

        // PUT {base}/api/products/{id}
        [ActionName("Default")]
        public HttpResponseMessage Put(int id, ProductModelUpdateRequest product)
        {
            return Invoke(Request, () =>
            {
                product.ProductID = id;
                var result = _service.UpdateProduct(product);
                var response = Request.CreatePutResponseFor(result);
                return response;
            });
        }

        // PUT {base}/api/products/{id}/mark-sold-out
        [ActionName("MarkSoldOut")]
        public HttpResponseMessage PutMarkSoldOut(int id)
        {
            return Invoke(Request, () =>
            {
                var result = _service.MarkProductSoldOut(id);
                var response = Request.CreatePutResponseFor(result);
                return response;
            });
        }

        // DELETE {base}/api/products/{id}
        [ActionName("Default")]
        public HttpResponseMessage Delete(int id)
        {
            return Invoke(Request, () =>
            {
                var result = _service.DeleteProduct(id);
                var response = Request.CreateDeleteResponseFor(result);
                return response;
            });
        }
 
    }
}