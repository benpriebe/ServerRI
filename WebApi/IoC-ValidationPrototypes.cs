//#region Using directives
//
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Data.Common;
//using System.Data.Entity;
//using System.Net.Http;
//using System.Reflection;
//using System.Web.Http;
//using System.Web.Http.Controllers;
//using System.Web.Http.Metadata;
//using System.Web.Http.Validation;
//using System.Web.ModelBinding;
//using Autofac;
//using Autofac.Integration.Mvc;
//using Autofac.Integration.WebApi;
//using Contracts.Data;
//using Core;
//using Core.IoCModules;
//using Data;
//using Data.Entities;
//using Models.Administration.Products;
//using Providers;
//using Providers.Administration;
//using Services;
//using WebApi.Controllers;
//using IModelBinder = System.Web.Http.ModelBinding.IModelBinder;
//using ModelBinderProvider = System.Web.Http.ModelBinding.ModelBinderProvider;
//using ModelBindingContext = System.Web.Http.ModelBinding.ModelBindingContext;
//
//#endregion
//
//
//namespace WebApi
//{
//    internal static class IoC
//    {
//        static IoC()
//        {
//            BuildContainer();
//        }
//
//        internal static IContainer Container { get; set; }
//
//        private static string DbConnectionString
//        {
//            get { return ConfigurationManager.ConnectionStrings["AWContext"].ConnectionString; }
//        }
//
//        internal static void BuildContainer()
//        {
//            var builder = new ContainerBuilder();
//
//            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
//            //builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
//            
//            // database
//            builder.Register(c => Database.DefaultConnectionFactory.CreateConnection(DbConnectionString))
//                .InstancePerApiRequest()
//                .Named<DbConnection>("DbConnection");
//
//            builder.Register(c =>
//            {
//                var connection = c.ResolveNamed<DbConnection>("DbConnection");
//                return new AWContext(connection, false);
//            })
//                .InstancePerApiRequest()
//                .As<DbContext>();
//
//            // services
//            builder.RegisterAssemblyTypes(typeof(BaseService).Assembly).Where(t => t.Name.EndsWith("Service"));
//
//            // providers
//            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
//
//            builder.RegisterType<ProductsProvider>().As<IProductsProvider>();
//            builder.RegisterType<EFProvider<Customer>>().As<IProvider<Customer>>();
//
//           
//            //TODO: 18-Mar-2013 - Ben - Figure out how to do this with chose authentication model.
//            // operationcontext - should only have one instance per thread/user
//            builder.RegisterInstance(new OperationContext(new UserDetails(1.ToString(), "To Do", "to.do")));
//
//            // logging
//            builder.RegisterModule<IoCLoggingModule>();
//
//            // filters
//            builder.RegisterType<FilterConfig.CommonLogErrorApiAttribute>();
//
//
//            // validators
////            builder.Register(c => Database.DefaultConnectionFactory.CreateConnection(DbConnectionString))
////                .Named<DbConnection>("ValidatorDbConnection");
////
////            builder.Register(c =>
////            {
////                var connection = c.ResolveNamed<DbConnection>("ValidatorDbConnection");
////                return new AWContext(connection, false);
////            })
////                .Named<DbContext>("ValidatorDbContext");
////
////            builder.Register(c => new UnitOfWork(c.ResolveNamed<DbContext>("ValidatorDbContext"), c.Resolve<Func<DbContext, IProductsProvider>>(), c.Resolve<Func<DbContext, IProvider<Customer>>>()))
////                .Named<IUnitOfWork>("ValidatorUnitOfWork");
////
//////            builder.Register(c => new CustomModelValidatorProvider(c.ResolveNamed<IUnitOfWork>("ValidatorUnitOfWork")))
//////                .As<ModelValidatorProvider>()
//////                .InstancePerApiControllerType(typeof(ProductsController));
////
////            builder.Register(c => new CustomModelBinderProvider(c.ResolveNamed<IUnitOfWork>("ValidatorUnitOfWork")))
////             .As<ModelBinderProvider>()
////             .InstancePerApiControllerType(typeof(ProductsController));
//            
//            Container = builder.Build();
//
//            // webapi controller resolver
//            var webApiDependencyResolver = new AutofacWebApiDependencyResolver(Container);
//            GlobalConfiguration.Configuration.DependencyResolver = webApiDependencyResolver;
//        }
//    }
//
//    public class CustomModelBinderProvider : ModelBinderProvider
//    {
//         private readonly IUnitOfWork _uow;
//
//        public CustomModelBinderProvider(IUnitOfWork uow)
//        {
//            _uow = uow;
//        }
//
//        public override IModelBinder GetBinder(HttpConfiguration configuration, Type modelType)
//        {
//            //var constructor = modelType == typeof(IUnitOfWorkable); 
//            var constructor = modelType.GetConstructor(new[]
//            {
//                typeof(IUnitOfWork)
//            });
//
//            return constructor  != null
//                ? new CustomModelBinder(_uow) 
//                : null;
//        }
//    }
//
//    public class CustomModelBinder : IModelBinder
//    {
//        private readonly IUnitOfWork _uow;
//
//        public CustomModelBinder(IUnitOfWork uow)
//        {
//            _uow = uow;
//        }
//
//        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
//        {
//            var obj = actionContext.ControllerContext.Request.Content.ReadAsAsync(bindingContext.ModelType);
//            IUnitOfWorkable model = obj.Result as IUnitOfWorkable;
//            model.UoW = _uow;
//            bindingContext.Model = model;
//            return true;
//        }
//    }
//
////    public class CustomModelValidatorProvider : ModelValidatorProvider
////    {
////        private readonly IUnitOfWork _uow;
////
////        public CustomModelValidatorProvider(IUnitOfWork uow)
////        {
////            _uow = uow;
////        }
////         
////        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, IEnumerable<ModelValidatorProvider> validatorProviders)
////        {
////            if (metadata.ModelType == typeof(ProductModelCreateRequest))
////            {
////                yield return new ProductModelValidator(_uow, validatorProviders);
////            }
////        }
////    }
////
////    public class ProductModelValidator : ModelValidator
////    {
////        private readonly IUnitOfWork _uow;
////
////        public ProductModelValidator(IUnitOfWork uow, IEnumerable<ModelValidatorProvider> validatorProviders) : base(validatorProviders)
////        {
////            _uow = uow;
////        }
////
////        public override IEnumerable<ModelValidationResult> Validate(ModelMetadata metadata, object container)
////        {
////            var model = metadata.Model as ProductModelCreateRequest;
////            if (true)
////            {
////                yield return new ModelValidationResult
////                  {
////                    MemberName = "Model.Member",
////                    Message = "Rating is required."
////                  };
////            }
////        }
////    }
//}