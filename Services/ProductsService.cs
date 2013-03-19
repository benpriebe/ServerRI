#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Api.Common;
using Common.Logging;
using Contracts.Data;
using Core;
using Core.Extensions;
using Data.Entities;
using Models.Administration.Products;
using Models.Resx;
using Providers;

#endregion


namespace Services
{
    public class ProductsService : BaseService
    {
        private readonly ILog _log;
        private readonly IExternalProvider _externalProvider;

        public ProductsService(OperationContext context, ILog log, Func<IUnitOfWork> uow, IExternalProvider externalProvider) : base(context, uow)
        {
            _log = log;
            _externalProvider = externalProvider;
        }

        public List<ValidationResult> Validate(params object[] models)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            foreach (var model in models)
            {
                var vc = new ValidationContext(model, null, null);
                Validator.TryValidateObject(model, vc, validationResults);
            }
            return validationResults;
        }

        public Result<int?> AddProduct(ProductModelCreateRequest product)
        {
            _log.Enter(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format("with product {0}", product.ToJson()));

            // validates the model using data annotations and the IValidatableObject interface method.
            var results = Validate(product);
            if (results.Any()) return Result<int?>.CreateValidationErrors(results.ToArray());

            // note: you might want to call out to some provider to do something. this is how you can do it!
            var widgets = _externalProvider.GetWidgets();

            var utcNow = DateTime.UtcNow;
            var entity = Mapper.Map<Product>(product);
            entity.ModifiedDate = utcNow;
            entity.SellStartDate = utcNow;

            try
            {
                using (var uow = UoW())
                {
                    // more complex validation - requires a database lookup.
                    if (uow.Products.GetAll().Any(p => p.ProductNumber == product.ProductNumber))
                    {
                        //TODO: 19-Mar-2013 - Ben - Structure resx files appropriately. This is for demonstration but wrong. Shouldn't depend on the Model class for this message.
                        return Result<int?>.CreateValidationErrors(new ValidationResult(String.Format(LocalizedErrors.ProductsService_AddProduct_ErrorCode1, product.ProductNumber), new[] { "ProductNumber" }));
                    }
                    
                    uow.Products.Add(entity);
                    uow.Commit();
                    _log.Info(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format(" product added - id = {0}", entity.ProductID));
                }
            }
            catch (ProviderException e)
            {
                _log.Exception(GetType(), MethodBase.GetCurrentMethod(), Context, e, e.Errors.ToJson());
                return ResultExtensions.Create<int?>(e, (int) ServiceMessages.Codes.ProviderError);
            }

            product.Id = entity.ProductID;

            _log.Exit(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format("with product {0}", product.ToJson()));
            return Result<int?>.Create(entity.ProductID);
        }

        public Result MarkProductSoldOut(int productId)
        {
            return UpdateProduct(new ProductModelUpdateRequest
            {
                ProductID = productId,
                StandardCost = 0 // this is how i'm representing sold out. dodgy i know. illustrative purposes only.
            });
        }

        public Result UpdateProduct(ProductModelUpdateRequest product)
        {
            _log.Enter(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format("with product {0}", product.ToJson()));

            // validates the model using data annotations and the IValidatableObject interface method.
            var results = Validate(product);
            if (results.Any()) return Result.CreateValidationErrors(results.ToArray());

            try
            {
                using (var uow = UoW())
                {
                    var entity = uow.Products.GetById(product.ProductID);

                    if (entity == null)
                    {
                        return Result.CreateNotFound<Product>(product.ProductID);
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
                return ResultExtensions.Create(e, (int) ServiceMessages.Codes.ProviderError);
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
                catch (NotFoundProviderException)
                {
                    return Result.CreateNotFound<Product>(productId);
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
                    ? Result<ProductModelResponse>.CreateNotFound<Product>(productId)
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
                _log.Exit(GetType(), MethodBase.GetCurrentMethod(), Context);
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