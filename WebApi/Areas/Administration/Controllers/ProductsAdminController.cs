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
using WebApi.Controllers;

#endregion


namespace WebApi.Areas.Administration.Controllers
{
    [AutofacControllerConfiguration]
    public class ProductsAdminController : ProductsController
    {
        private readonly ProductsService _service;

        public ProductsAdminController(OperationContext context, ILog log, ProductsService service) : base(context, log, service)
        {
            _service = service;
        }

        // POST {base}/api/admin/products
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

        // PUT {base}/api/admin/products/{id}
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

        // PUT {base}/api/admin/products/{id}/mark-sold-out
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

        // DELETE {base}/api/admin/products/{id}
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