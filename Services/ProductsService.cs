#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Api.Common;
using Common.Logging;
using Contracts.Data;
using Core;
using Core.Extensions;
using Data.Entities;
using Models.Products;
using Providers;
using Services.Resx;

#endregion


namespace Services
{
    public class ProductsService : BaseService
    {
        private readonly IExternalProvider _externalProvider;

        public ProductsService(OperationContext context, ILog log, Func<IUnitOfWork> uow, IExternalProvider externalProvider) : base(context, log, uow)
        {
            _externalProvider = externalProvider;
        }

        public Result<int?> AddProduct(ProductModelCreateRequest product)
        {
            var watch = new Stopwatch();
            watch.Start();

            using (new OperationLogger(Log, m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format("with product {0}", product.ToJson()))))
            {
                Console.Out.WriteLine("A - " + watch.ElapsedMilliseconds);

                // validates the model using data annotations and the IValidatableObject interface method.
                var results = Validate(product);

                if (results.Any()) return Result<int?>.CreateValidationErrors(results.ToArray());

                Console.Out.WriteLine("B - " + watch.ElapsedMilliseconds);

                // note: you might want to call out to some provider to do something. this is how you can do it!
                var widgets = _externalProvider.GetWidgets();

                var utcNow = DateTime.UtcNow;
                var entity = Mapper.Map<Product>(product);
                entity.ModifiedDate = utcNow;
                entity.SellStartDate = utcNow;

                Console.Out.WriteLine("C - " + watch.ElapsedMilliseconds);

                try
                {
                    using (var uow = UoW())
                    {
                        Console.Out.WriteLine("D - " + watch.ElapsedMilliseconds);

                        // more complex validation - requires a database lookup.
                        if (uow.Products.GetAll().Any(p => p.ProductNumber == product.ProductNumber))
                        {
                            Console.Out.WriteLine("E - " + watch.ElapsedMilliseconds);
                            return Result<int?>.CreateValidationErrors(new ValidationResult(String.Format(Strings.ProductsService_AddProduct_ValidationError_1, product.ProductNumber), new[]
                            {
                                "ProductNumber"
                            }));
                        }

                        Console.Out.WriteLine("F - " + watch.ElapsedMilliseconds);

                        uow.Products.Add(entity);
                        uow.Commit();

                        Console.Out.WriteLine("G - " + watch.ElapsedMilliseconds);

                        Log.Info(m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format(" product added - id = {0}", entity.ProductID)));

                        Console.Out.WriteLine("H - " + watch.ElapsedMilliseconds);
                    }
                }
                catch (ProviderException e)
                {
                    Console.Out.WriteLine("I - " + watch.ElapsedMilliseconds);
                    Log.Exception(m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context, e.Errors.ToJson()), e);
                    Console.Out.WriteLine("J - " + watch.ElapsedMilliseconds);
                    return ResultExtensions.Create<int?>(e, (int) ServiceMessages.Codes.ProviderError);
                }

                product.Id = entity.ProductID;
                Console.Out.WriteLine("K - " + watch.ElapsedMilliseconds);
                return Result<int?>.Create(entity.ProductID);
            }
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
            var watch = new Stopwatch();
            watch.Start();

            using (new OperationLogger(Log, m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format("with product {0}", product.ToJson()))))
            {
                Console.Out.WriteLine("A - " + watch.ElapsedMilliseconds);

                // validates the model using data annotations and the IValidatableObject interface method.
                var results = Validate(product);
                if (results.Any()) return Result.CreateValidationErrors(results.ToArray());

                Console.Out.WriteLine("B - " + watch.ElapsedMilliseconds);

                try
                {
                    using (var uow = UoW())
                    {
                        Console.Out.WriteLine("C - " + watch.ElapsedMilliseconds);

                        var entity = uow.Products.GetById(product.ProductID);

                        Console.Out.WriteLine("D - " + watch.ElapsedMilliseconds);

                        if (entity == null)
                        {
                            return Result.CreateNotFound<Product>(product.ProductID);
                        }
                        entity.ListPrice = product.ListPrice;
                        entity.StandardCost = product.StandardCost;
                        entity.ModifiedDate = DateTime.UtcNow;

                        uow.Commit();

                        Console.Out.WriteLine("E - " + watch.ElapsedMilliseconds);

                        Log.Info(m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format(" product updated - id = {0}", entity.ProductID)));

                        Console.Out.WriteLine("F - " + watch.ElapsedMilliseconds);
                    }
                }
                catch (ProviderException e)
                {
                    Console.Out.WriteLine("G - " + watch.ElapsedMilliseconds);
                    Log.Exception(m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context, e.Errors.ToJson()), e);
                    Console.Out.WriteLine("H - " + watch.ElapsedMilliseconds);
                    return ResultExtensions.Create(e, (int) ServiceMessages.Codes.ProviderError);
                }

                Console.Out.WriteLine("I - " + watch.ElapsedMilliseconds);

                Console.Out.WriteLine("J - " + watch.ElapsedMilliseconds);
                return Result.CreateEmpty();
            }
        }


        public Result DeleteProduct(int productId)
        {
            using (new OperationLogger(Log, m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format("with productId {0}", productId))))
            {
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

                    return Result.CreateEmpty();
                }
            }
        }

        public Result<ProductModelResponse> GetProductById(int productId)
        {
            using (new OperationLogger(Log, m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format("with productId {0}", productId))))
            {
                using (var uow = UoW())
                {
                    var entity = uow.Products.GetById(productId);


                    return entity == null
                        ? Result<ProductModelResponse>.CreateNotFound<Product>(productId)
                        : Result<ProductModelResponse>.Create(Mapper.Map<ProductModelResponse>(entity));
                }
            }
        }

        public Result<IList<ProductModel>> GetProducts()
        {
            return GetProducts(null);
        }

        public Result<IList<ProductModel>> GetProducts(ProductModelFilterRequest filter)
        {
            using (new OperationLogger(Log, m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context)))
            {
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
                    return Result<IList<ProductModel>>.Create(Mapper.Map<IList<ProductModel>>(entities));
                }
            }
        }

        public Result<IList<ProductModelResponse>> GetProductsWithDetails()
        {
            using (new OperationLogger(Log, m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context)))
            {
                using (var uow = UoW())
                {
                    IList<Product> entities = uow.Products.GetProductsWithDetails().ToList();
                    return Result<IList<ProductModelResponse>>.Create(Mapper.Map<IList<ProductModelResponse>>(entities));
                }
            }
        }
    }
}