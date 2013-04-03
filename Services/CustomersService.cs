#region Using directives

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using Api.Common;
using Common.Logging;
using Contracts.Data;
using Core;
using Core.Extensions;
using Data.Entities;
using Models.Customers;
using Providers;

#endregion


namespace Services
{
    public class CustomersService : BaseService
    {
        public CustomersService(OperationContext context, ILog log, Func<IUnitOfWork> uow) : base(context, log, uow)
        {
        }

        public Result<int?> AddCustomer(CustomerModel customer)
        {
            using (new OperationLogger(Log, m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format("with customer {0}", customer.ToJson()))))
            {
                // validates the model using data annotations and the IValidatableObject interface method.
                var results = Validate(customer);

                if (results.Any()) return Result<int?>.CreateValidationErrors(results.ToArray());

                var entity = Mapper.Map<Customer>(customer);
                UpdateModifiedDateAndRowGuids(entity);
                
                try
                {
                    using (var uow = UoW())
                    {
                        uow.Customers.Add(entity);
                        uow.Commit();

                        Log.Info(m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format(" customer added - id = {0}", entity.CustomerID)));
                    }
                }
                catch (ProviderException e)
                {
                    Log.Exception(m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context, e.Errors.ToJson()), e);
                    return ResultExtensions.Create<int?>(e, (int) ServiceMessages.Codes.ProviderError);
                }

                customer.CustomerID = entity.CustomerID;
                return Result<int?>.Create(entity.CustomerID);
            }
        }


        public Result UpdateCustomer(CustomerModel customer)
        {
            using (new OperationLogger(Log, m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format("with customer {0}", customer.ToJson()))))
            {
                // validates the model using data annotations and the IValidatableObject interface method.
                var results = Validate(customer);
                if (results.Any()) return Result.CreateValidationErrors(results.ToArray());

                try
                {
                    using (var uow = UoW())
                    {
                        var entity = uow.Customers.GetAll()
                            .Include(c => c.CustomerAddresses.Select(ca => ca.Address))
                            .SingleOrDefault(c => c.CustomerID == customer.CustomerID);

                        if (entity == null)
                        {
                            return Result.CreateNotFound<Customer>(customer.CustomerID);
                        }

                        Mapper.Map(customer, entity);
                        UpdateModifiedDateAndRowGuids(entity);

                        uow.Commit();
                        Log.Info(m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format(" customer updated - id = {0}", entity.CustomerID)));
                    }
                }
                catch (ProviderException e)
                {
                    Log.Exception(m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context, e.Errors.ToJson()), e);
                    return ResultExtensions.Create(e, (int) ServiceMessages.Codes.ProviderError);
                }

                return Result.CreateEmpty();
            }
        }

        private static void UpdateModifiedDateAndRowGuids(Customer entity)
        {
            var utcNow = DateTime.UtcNow;
            entity.ModifiedDate = utcNow;
            entity.rowguid = Guid.NewGuid();
            foreach (var custAddress in entity.CustomerAddresses)
            {
                custAddress.ModifiedDate = utcNow;
                custAddress.rowguid = Guid.NewGuid();
                if (custAddress.Address != null)
                {
                    custAddress.Address.ModifiedDate = utcNow;
                    custAddress.Address.rowguid = Guid.NewGuid();
                }
            }
        }
    }
}