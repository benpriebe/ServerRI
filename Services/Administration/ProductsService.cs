#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Api.Common;
using Common.Logging;
using Contracts.Data;
using Core;
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

        public ProductsService(OperationContext context, ILog log, Func<IUnitOfWork> uow) : base(context, uow)
        {
            _log = log;
        }

        public Result<int?> AddProduct(ProductModelCreateRequest product)
        {
            _log.Enter(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format("with product {0}", product.ToJson()));

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
                    _log.Info(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format(" product added - id = {0}", entity.ProductID));
                }
            }
            catch (ProviderException e)
            {
                _log.Exception(GetType(), MethodBase.GetCurrentMethod(), Context, e, e.Errors.ToJson());
                return ResultExtensions.Create<int?>(e, (int)ServiceMessages.Codes.ProviderError);
            }

            product.Id = entity.ProductID;

            _log.Exit(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format("with product {0}", product.ToJson()));
            return Result<int?>.Create(entity.ProductID);
        }

        public Result MarkProductSoldOut(int productId)
        {
            return UpdateProduct(new ProductModelUpdateRequest()
            {
                ProductID = productId,
                StandardCost = 0 // this is how i'm representing sold out. dodgy i know. illustrative purposes only.
            });
        }

        public Result UpdateProduct(ProductModelUpdateRequest product)
        {
            _log.Enter(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format("with product {0}", product.ToJson()));

            try
            {
                using (var uow = UoW())
                {
                    var entity = uow.Products.GetById(product.ProductID);
                    
                    if (entity == null)
                    {
                        return Result.Create(ServiceMessages.ProductNotFound(product.ProductID));
                    }
                    entity.ListPrice = product.ListPrice;
                    entity.StandardCost = product.StandardCost;
                    entity.ModifiedDate = DateTime.UtcNow;
                    
                    uow.Commit();
                    _log.Info(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format(" product updated - id = {0}", entity.ProductID));
                }
            }
            catch (ProviderException e)
            {
                _log.Exception(GetType(), MethodBase.GetCurrentMethod(), Context, e, e.Errors.ToJson());
                return ResultExtensions.Create(e, (int)ServiceMessages.Codes.ProviderError);
            }

            _log.Exit(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format("with product {0}", product.ToJson()));
            return Result.CreateEmpty();
        }


        public Result DeleteProduct(int productId)
        {
            _log.Enter(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format("with productId {0}", productId));

            using (var uow = UoW())
            {
                try
                {
                    uow.Products.Delete(productId);
                    uow.Commit();
                }
                catch (NotFoundProviderException e)
                {
                    return Result.Create(ServiceMessages.ProductNotFound(productId));
                }
                _log.Exit(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format("with productId {0}", productId));
                return Result.CreateEmpty();
            }
        }

        public Result<ProductModelResponse> GetProductById(int productId)
        {
            _log.Enter(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format("with productId {0}", productId));

            using (var uow = UoW())
            {
                var entity = uow.Products.GetById(productId);
                _log.Exit(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format("with productId {0}", productId));

                return entity == null
                    ? Result<ProductModelResponse>.Create(ServiceMessages.ProductNotFound(productId)) 
                    : Result<ProductModelResponse>.Create(Mapper.Map<ProductModelResponse>(entity));
            }
        }

        public Result<IList<ProductModel>> GetProducts()
        {
            return GetProducts(null);
        }

        public Result<IList<ProductModel>> GetProducts(ProductModelFilterRequest filter)
        {
            _log.Enter(GetType(), MethodBase.GetCurrentMethod(), Context);

            using (var uow = UoW())
            {
                var query = uow.Products.GetAll();
                //TODO: 17-Mar-2013 - Ben - make this more generic and reusable
                if (filter != null)
                {
                    query = !String.IsNullOrWhiteSpace(filter.ProductName) ? query.Where(p => p.Name.StartsWith(filter.ProductName)) : query;
                    query = query.OrderBy(p => p.ProductID);
                    query = filter.Skip.HasValue ? query.Skip(filter.Skip.Value) : query;
                    query = filter.Top.HasValue ? query.Take(filter.Top.Value) : query;
                }
                var entities = query.ToList();
                _log.Exit(GetType(), MethodBase.GetCurrentMethod(),Context);
                return Result<IList<ProductModel>>.Create(Mapper.Map<IList<ProductModel>>(entities));
            }
        }

        public Result<IList<ProductModelResponse>> GetProductsWithDetails()
        {
            _log.Enter(GetType(), MethodBase.GetCurrentMethod(), Context);

            using (var uow = UoW())
            {
                IList<Product> entities = uow.Products.GetProductsWithDetails().ToList();
                _log.Exit(GetType(), MethodBase.GetCurrentMethod(), Context);
                return Result<IList<ProductModelResponse>>.Create(Mapper.Map<IList<ProductModelResponse>>(entities));
            }
        }
    }
}