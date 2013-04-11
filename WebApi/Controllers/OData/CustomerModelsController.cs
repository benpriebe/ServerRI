#region Using directives

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using Autofac.Integration.WebApi;
using Common.Logging;
using Core;
using Models.Customers;
using Models.Products;
using Services;

#endregion


namespace WebApi.Controllers.OData
{
    [AutofacControllerConfiguration]
    public class CustomerModelsController : EntitySetController<CustomerModelResponse, int>
    {
        private readonly CustomersService _service;
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

        public CustomerModelsController(OperationContext context, ILog log, CustomersService service)
        {
            _context = context;
            _log = log;
            _service = service;
        }

        public override IQueryable<CustomerModelResponse> Get()
        {
            return _service.QueryCustomers().Value;
        }

        protected override CustomerModelResponse GetEntityByKey(int key)
        {
            return _service.GetCustomerById(key).Value;
        }

        [System.Web.Http.Queryable]
        public IQueryable<IEnumerable<CustomerAddressModel>> GetAddressesFromCustomer([FromODataUri] int key)
        {
            return _service.QueryCustomers().Value.Where(c => c.CustomerID == key).Select(c => c.CustomerAddresses);
        }
    }
}