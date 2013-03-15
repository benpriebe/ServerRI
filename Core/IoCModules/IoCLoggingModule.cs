#region Using directives

using System;
using System.Linq;
using Autofac;
using Autofac.Core;
using Common.Logging;

#endregion


namespace Core.IoCModules
{
    public class IoCLoggingModule : Module
    {
        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            base.AttachToComponentRegistration(componentRegistry, registration);

            registration.Preparing += (sender, args) =>
            {
                Type t = args.Component.Activator.LimitType;
                args.Parameters = args.Parameters.Union(new[]
                {
                    new ResolvedParameter(
                        (parameter, component) => parameter.ParameterType == typeof(ILog),
                        (parameter, component) => LogManager.GetLogger(t)
                        )
                });
            };
        }
    }
}