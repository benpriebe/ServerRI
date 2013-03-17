#region Using directives

using System;
using System.Net.Http;
using System.Transactions;
using System.Web.Http;
using Api.Web.Extensions;
using Common.Logging;
using Models.Administration.Products;
using Services.Administration;

#endregion


namespace WebApi.Controllers
{
    public class ProductsController : ApiController
    {
        private readonly bool _commit = true;
        private readonly ILog _log;
        private readonly ProductsService _service;

        public ProductsController(ILog log, ProductsService service)
        {
            _log = log;
            _service = service;
#if DEBUG
            _commit = false;
#endif
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
            using (var ts = new TransactionScope())
            {
                var result = _service.AddProduct(product);
                var response = Request.CreatePostResponseFor(result, WebApiConfig.ProductsRouteName, new { id = result.Value != null ? result.Value.ToString() : string.Empty});

                if (_commit) ts.Complete();
                return response;
            }
        }

        // PUT {base}/admin/products/{id}
        [ActionName("Default")]
        public HttpResponseMessage Put(int id, ProductModelUpdateRequest product)
        {
            using (var ts = new TransactionScope())
            {
                product.ProductID = id;
                var result = _service.UpdateProduct(product);
                var response = Request.CreatePutResponseFor(result);
                if (_commit) ts.Complete();
                return response;
            }
        }

        // PUT {base}/admin/products/{id}/mark-sold-out
        [ActionName("MarkSoldOut")]
        public HttpResponseMessage PutMarkSoldOut(int id)
        {
            using (var ts = new TransactionScope())
            {
                var result = _service.MarkProductSoldOut(id);
                var response = Request.CreatePutResponseFor(result);
                if (_commit) ts.Complete();
                return response;
            }
        }

        // DELETE {base}/admin/products/{id}
        [ActionName("Default")]
        public HttpResponseMessage Delete(int id)
        {
            using (var ts = new TransactionScope())
            {
                var result = _service.DeleteProduct(id);
                var response = Request.CreateDeleteResponseFor(result);
                if (_commit) ts.Complete();
                return response;
            }
        }
    }
}