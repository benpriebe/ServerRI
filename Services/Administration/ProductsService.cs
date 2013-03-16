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
    public class ProductsService : BaseService
    {
        private readonly ILog _log;

        public ProductsService(ILog log, Func<IUnitOfWork> uow) : base(uow)
        {
            _log = log;
        }

        public Result AddProduct(ProductModelRequest product)
        {
            _log.Enter(GetType(), MethodBase.GetCurrentMethod(), String.Format("with product {0}", product.ToJson()));

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
                    _log.Info(GetType(), MethodBase.GetCurrentMethod(), String.Format(" product added - id = {0}", entity.ProductID));
                }
            }
            catch (ProviderException e)
            {

                try
                {
                    _log.Exception(GetType(), MethodBase.GetCurrentMethod(), e, e.Errors.ToJson());
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
                return ResultExtensions.Create(e, 666);
                //todo: 15-Mar-2013 - Ben - when implementing the web.api make sure all messagelevel.errors get turned into HttpStatusCode.500
            }

            product.Id = entity.ProductID;

            _log.Exit(GetType(), MethodBase.GetCurrentMethod(), String.Format("with product {0}", product.ToJson()));
            return Result.CreateEmpty();
        }


        public Result DeleteProduct(int productId)
        {
            _log.Enter(GetType(), MethodBase.GetCurrentMethod(), String.Format("with productId {0}", productId));

            using (var uow = UoW())
            {
                uow.Products.Delete(productId);
                uow.Commit();

                _log.Exit(GetType(), MethodBase.GetCurrentMethod(), String.Format("with productId {0}", productId));
                return Result.CreateEmpty();
            }
        }

        public Result<ProductModelResponse> GetProductById(int productId)
        {
            _log.Enter(GetType(), MethodBase.GetCurrentMethod(), String.Format("with productId {0}", productId));

            using (var uow = UoW())
            {
                var entity = uow.Products.GetById(productId);
                _log.Exit(GetType(), MethodBase.GetCurrentMethod(), String.Format("with productId {0}", productId));
                return entity == null
                    ? Result<ProductModelResponse>.Create(ServiceMessages.ProductNotFound) //TODO: 15-Mar-2013 - Ben - turn this into a 404 on the web api layer.
                    : Result<ProductModelResponse>.Create(Mapper.Map<ProductModelResponse>(entity));
            }
        }

        public Result<IList<ProductModel>> GetProducts()
        {
            _log.Enter(GetType(), MethodBase.GetCurrentMethod());

            using (var uow = UoW())
            {
                IList<Product> entities = uow.Products.GetAll().ToList();
                _log.Exit(GetType(), MethodBase.GetCurrentMethod());
                return Result<IList<ProductModel>>.Create(Mapper.Map<IList<ProductModel>>(entities));
            }
        }

        public Result<IList<ProductModelResponse>> GetProductsWithDetails()
        {
            _log.Enter(GetType(), MethodBase.GetCurrentMethod());

            using (var uow = UoW())
            {
                IList<Product> entities = uow.Products.GetProductsWithDetails().ToList();
                _log.Exit(GetType(), MethodBase.GetCurrentMethod());
                return Result<IList<ProductModelResponse>>.Create(Mapper.Map<IList<ProductModelResponse>>(entities));
            }
        }
    }
}