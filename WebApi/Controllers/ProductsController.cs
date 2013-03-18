#region Using directives

using System;
using System.Net.Http;
using System.Web.Http;
using Api.Web.Extensions;
using Common.Logging;
using Core;
using Models.Administration.Products;
using Services.Administration;

#endregion


namespace WebApi.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly ProductsService _service;

        public ProductsController(OperationContext context, ILog log, ProductsService service) : base(context, log)
        {
            _service = service;
        }

        // GET {base}/admin/products/{id}
        [ActionName("Default")]
        public HttpResponseMessage Get(int id)
        {
            var result = _service.GetProductById(id);
            return Request.CreateGetResponseFor(result);
        }

        // GET {base}/admin/products?productname={name}&top={top}&skip={skip}
        [ActionName("Default")]
        public HttpResponseMessage Get([FromUri] ProductModelFilterRequest filter)
        {
            if (filter.ProductName == "error") // note: do not do this in production env.
                throw new Exception("This is to show how exceptions are handled by default in WebAPI");

            var result = _service.GetProducts(filter);
            return Request.CreateGetResponseFor(result);
        }

        // POST {base}/admin/products
        [ActionName("Default")]
        public HttpResponseMessage Post(ProductModelCreateRequest product)
        {
            return Invoke(() =>
            {
                var result = _service.AddProduct(product);
                var response = Request.CreatePostResponseFor(result, WebApiConfig.ProductsRouteName, new
                {
                    id = result.Value != null ? result.Value.ToString() : string.Empty
                });
                return response;
            });
        }

        // PUT {base}/admin/products/{id}
        [ActionName("Default")]
        public HttpResponseMessage Put(int id, ProductModelUpdateRequest product)
        {
            return Invoke(() =>
            {
                product.ProductID = id;
                var result = _service.UpdateProduct(product);
                var response = Request.CreatePutResponseFor(result);
                return response;
            });
        }

        // PUT {base}/admin/products/{id}/mark-sold-out
        [ActionName("MarkSoldOut")]
        public HttpResponseMessage PutMarkSoldOut(int id)
        {
            return Invoke(() =>
            {
                var result = _service.MarkProductSoldOut(id);
                var response = Request.CreatePutResponseFor(result);
                return response;
            });
        }

        // DELETE {base}/admin/products/{id}
        [ActionName("Default")]
        public HttpResponseMessage Delete(int id)
        {
            return Invoke(() =>
            {
                var result = _service.DeleteProduct(id);
                var response = Request.CreateDeleteResponseFor(result);
                return response;
            });
        }
    }
}