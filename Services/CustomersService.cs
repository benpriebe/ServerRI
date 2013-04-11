#region Using directives

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Api.Common;
using Common.Logging;
using Contracts.Data;
using Core;
using Core.Extensions;
using Data.Entities;
using Models.Addresses;
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

        public Result<CustomerModelResponse> GetCustomerById(int customerId)
        {
            using (new OperationLogger(Log, m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format("with productId {0}", customerId))))
            {
                using (var uow = UoW())
                {
                    var entity = uow.Customers.GetById(customerId);
                    return entity == null
                        ? Result<CustomerModelResponse>.CreateNotFound<Customer>(customerId)
                        : Result<CustomerModelResponse>.Create(Mapper.Map<CustomerModelResponse>(entity));
                }
            }
        }

        public Result<IQueryable<CustomerModelResponse>> QueryCustomers(bool withAddresses = false)
        {
            using (new OperationLogger(Log, m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context)))
            {
                var uow = UoW(); // note: we cannot dispose of the UnitOfWork here as we need it to stay alive until the query is executed.

                // note: while it would be nice to use auto-model to do the entity-to-model projection; auto-mapper isn't capable of doing this.
                var query = uow.Customers.GetAll().Select(withAddresses ? GetCustomersWithAddresses() : GetCustomers());
                return Result<IQueryable<CustomerModelResponse>>.Create(query);
            }
        }


        public Result<IQueryable<CustomerAddressModel>> QueryCustomerAddresses(int customerId)
        {
            using (new OperationLogger(Log, m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context)))
            {
                var uow = UoW(); // note: we cannot dispose of the UnitOfWork here as we need it to stay alive until the query is executed.

                // note: while it would be nice to use auto-model to do the entity-to-model projection; auto-mapper isn't capable of doing this.
                var query = uow.Customers.GetAll().Where(c => c.CustomerID == customerId).SelectMany(entity => entity.CustomerAddresses.Select(e => new CustomerAddressModel
                {
                    AddressID = e.AddressID,
                    AddressType = e.AddressType,
                    Address = new AddressModel
                    {
                        AddressID = e.Address.AddressID,
                        AddressLine1 = e.Address.AddressLine1,
                        AddressLine2 = e.Address.AddressLine2,
                        City = e.Address.City,
                        CountryRegion = e.Address.CountryRegion,
                        StateProvince = e.Address.StateProvince,
                        PostalCode = e.Address.PostalCode
                    }
                }));
                return Result<IQueryable<CustomerAddressModel>>.Create(query);
            }
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

        private Expression<Func<Customer, CustomerModelResponse>> GetCustomersWithAddresses()
        {
            return entity => new CustomerModelResponse
            {
                CustomerID = entity.CustomerID,
                FirstName = entity.FirstName,
                MiddleName = entity.MiddleName,
                Surname = entity.LastName,
                CompanyName = entity.CompanyName,
                EmailAddress = entity.EmailAddress,
                Title = entity.Title,
                Phone = entity.Phone,
                CustomerAddresses = entity.CustomerAddresses.Select(e => new CustomerAddressModel()
                {
                    AddressID = e.AddressID,
                    AddressType = e.AddressType,
                    Address = new AddressModel
                    {
                        AddressID = e.Address.AddressID,
                        AddressLine1 = e.Address.AddressLine1,
                        AddressLine2 = e.Address.AddressLine2,
                        City = e.Address.City,
                        CountryRegion = e.Address.CountryRegion,
                        StateProvince = e.Address.StateProvince,
                        PostalCode = e.Address.PostalCode
                    }
                })
            };
        }

        private Expression<Func<Customer, CustomerModelResponse>> GetCustomers()
        {
            return entity => new CustomerModelResponse
            {
                CustomerID = entity.CustomerID,
                FirstName = entity.FirstName,
                MiddleName = entity.MiddleName,
                Surname = entity.LastName,
                CompanyName = entity.CompanyName,
                EmailAddress = entity.EmailAddress,
                Title = entity.Title,
                Phone = entity.Phone,
            };
        }
    }
}