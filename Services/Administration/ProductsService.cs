#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Api.Common;
using Common.Logging;
using Contracts.Data;
using Core.Extensions;
using Data.Entities;
using Models.Administration.Products;
using Providers;

#endregion


namespace Services.Administration
{
    /// <summary>
    /// todo: figure exception handling
    /// todo: figure out logging.
    /// </summary>
    public class ProductsService : BaseService
    {
        private readonly ILog _log;

        public ProductsService(ILog log, Func<IUnitOfWork> uow) : base(uow)
        {
            _log = log;
        }

        public Result AddProduct(ProductModelRequest product)
        {
            _log.Info(m => m.Invoke("{0}:{1} - Entered with product {2}", GetType().Name, MethodBase.GetCurrentMethod().Name, product.ToJson()));

            var utcNow = DateTime.UtcNow;
            var entity = Mapper.Map<Product>(product);
            entity.ModifiedDate = utcNow;
            entity.SellStartDate = utcNow;

            try
            {
                using (var uow = UoW())
                {
                    uow.Products.Add(entity);
                    uow.Commit();
                }
            }
            catch (ProviderException e)
            {
                _log.Error(m => m.Invoke("{0}:{1} - ProviderException {2}", GetType().Name, MethodBase.GetCurrentMethod().Name, e.Errors.ToJson()), e);
                //TODO: 15-Mar-2013 - Ben - when implementing the web.api make sure all messagelevel.errors get turned into HttpStatusCode.500
                return Result.Create(new Message(MessageLevel.Error, 500, "RESX.Key", e.Errors.ToJson()));
            }

            product.Id = entity.ProductID;
            return Result.CreateEmpty();
        }

        public Result DeleteProduct(int productId)
        {
            _log.Info(m => m.Invoke("{0}:{1} - Entered with productId - {2}", GetType().Name, MethodBase.GetCurrentMethod().Name, productId));

            using (var uow = UoW())
            {
                uow.Products.Delete(productId);
                uow.Commit();
                return Result.CreateEmpty();
            }
        }

        public Result<ProductModelResponse> GetProductById(int productId)
        {
            _log.Info(m => m.Invoke("{0}:{1} - Entered with productId - {2}", GetType().Name, MethodBase.GetCurrentMethod().Name, productId));

            using (var uow = UoW())
            {
                var entity = uow.Products.GetById(productId);
                return entity == null
                    ? Result<ProductModelResponse>.Create(ServiceMessages.ProductNotFound) //TODO: 15-Mar-2013 - Ben - turn this into a 404 on the web api layer.
                    : Result<ProductModelResponse>.Create(Mapper.Map<ProductModelResponse>(entity));
            }
        }

        public Result<IList<ProductModel>> GetProducts()
        {
            _log.Info(m => m.Invoke("{0}:{1} - Entered", GetType().Name, MethodBase.GetCurrentMethod().Name));

            using (var uow = UoW())
            {
                IList<Product> entities = uow.Products.GetAll().ToList();
                return Result<IList<ProductModel>>.Create(Mapper.Map<IList<ProductModel>>(entities));
            }
        }

        public Result<IList<ProductModelResponse>> GetProductsWithDetails()
        {
            _log.Info(m => m.Invoke("{0}:{1} - Entered", GetType().Name, MethodBase.GetCurrentMethod().Name));

            using (var uow = UoW())
            {
                IList<Product> entities = uow.Products.GetProductsWithDetails().ToList();
                return Result<IList<ProductModelResponse>>.Create(Mapper.Map<IList<ProductModelResponse>>(entities));
            }
        }
    }
}