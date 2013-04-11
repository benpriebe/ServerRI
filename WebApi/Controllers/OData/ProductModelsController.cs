#region Using directives

using System.Collections.Generic;
using System.Linq;
using System.Web.Http.OData;
using Autofac.Integration.WebApi;
using Common.Logging;
using Core;
using Models.Products;
using Services;

#endregion


namespace WebApi.Controllers.OData
{
    [AutofacControllerConfiguration]
    public class ProductModelsController : EntitySetController<ProductModel, int>
    {
        private readonly ProductsService _service;
        private readonly OperationContext _context;
        private readonly ILog _log;

        public ILog Log
        {
            get { return _log; }
        }

        public OperationContext Context
        {
            get { return _context; }
        }

        public ProductModelsController(OperationContext context, ILog log, ProductsService service)
        {
            _context = context;
            _log = log;
            _service = service;
        }

        public override IQueryable<ProductModel> Get()
        {
            return _service.QueryProducts().Value;
        }

        protected override ProductModel GetEntityByKey(int key)
        {
            return _service.GetProductById(key).Value;
        }
    }
}