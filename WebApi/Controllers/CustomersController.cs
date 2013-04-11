#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using Api.Common;
using Api.Web.Extensions;
using Autofac.Integration.WebApi;
using Common.Logging;
using Core;
using Models.Customers;
using Models.Products;
using Services;

#endregion

namespace WebApi.Controllers
{
    [AutofacControllerConfiguration]
    public class CustomersController : BaseApiController
    {
        private readonly CustomersService _service;

        public CustomersController(OperationContext context, ILog log, CustomersService service) : base(context, log)
        {
            _service = service;
        }

        // GET {base}/api/customers/{id}
        [ActionName("Default")]
        public HttpResponseMessage Get(int id)
        {
            var result = _service.GetCustomerById(id);
            return Request.CreateGetResponseFor(result);
        }

        // GET {base}/api/customers/?$filter=...insert odata query... 
        // OData Query Syntax -  http://msdn.microsoft.com/en-us/library/ff478141.aspx, http://www.odata.org/documentation/odata-v2-documentation/uri-conventions/
        // NOTE: you can limit the query constraints by adding constraints to this attribute. http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/supporting-odata-query-options
        [Queryable]
        [ActionName("Default")]
        public HttpResponseMessage Get(bool includeAddresses = false)
        {
            var result = _service.QueryCustomers(includeAddresses);
            return Request.CreateGetResponseFor(result);
        }

        // GET {base}/api/customers/{id}/addresses
        // OData Query Syntax -  http://msdn.microsoft.com/en-us/library/ff478141.aspx, http://www.odata.org/documentation/odata-v2-documentation/uri-conventions/
        // NOTE: you can limit the query constraints by adding constraints to this attribute. http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/supporting-odata-query-options
        [Queryable]
        [ActionName("AddressesForCustomer")]
        public HttpResponseMessage GetAddressesFromCustomer(int id)
        {
            var results = _service.QueryCustomerAddresses(id).Value;
            var result = Result<IQueryable<CustomerAddressModel>>.Create(results);
            return Request.CreateGetResponseFor(result);
        }
 
 
    }
}