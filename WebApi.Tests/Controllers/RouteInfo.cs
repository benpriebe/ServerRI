using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace WebApi.Tests.Controllers
{
    public class RouteInfo
    {
        public Type Controller { get; set; }
        public string Action { get; set; }
        public Collection<HttpParameterDescriptor> Params { get; set; }
        public IHttpRouteData RouteData { get; set; }

        public static RouteInfo Verify(HttpConfiguration config, HttpMethod httpMethod, string uri, Type expectedController, string action)
        {
            var request = new HttpRequestMessage(httpMethod, uri);
            var routeInfo = RouteRequest(config, request);
            Verify(expectedController, action, routeInfo);
            return routeInfo;
        }

        public void VerifyRouteData(string key, string expectedValue)
        {
            Assert.AreEqual(expectedValue, RouteData.Values[key]);
        }

        private static void Verify(Type expectedController, string expectedAction, RouteInfo routeInfo)
        {
            Assert.AreEqual(expectedController, routeInfo.Controller);
            Assert.AreEqual(expectedAction, routeInfo.Action);
        }

        private static RouteInfo RouteRequest(HttpConfiguration config, HttpRequestMessage request)
        {
            var mockRouteData = new Mock<IHttpRouteData>();

            // create context
            var controllerContext = new HttpControllerContext(config, mockRouteData.Object, request);

            // get route data
            var routeData = config.Routes.GetRouteData(request);
            RemoveOptionalRoutingParameters(routeData.Values);

            request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;
            controllerContext.RouteData = routeData;

            // get controller type
            var controllerDescriptor = new DefaultHttpControllerSelector(config).SelectController(request);
            controllerContext.ControllerDescriptor = controllerDescriptor;

            // get action name
            var actionMapping = new ApiControllerActionSelector().SelectAction(controllerContext);

            return new RouteInfo
            {
                Controller = controllerDescriptor.ControllerType,
                Action = actionMapping.ActionName,
                Params = actionMapping.GetParameters(),
                RouteData = routeData,
            };
        }

        private static void RemoveOptionalRoutingParameters(IDictionary<string, object> routeValues)
        {
            var optionalParams = routeValues
                .Where(x => x.Value == RouteParameter.Optional)
                .Select(x => x.Key)
                .ToList();

            foreach (var key in optionalParams)
            {
                routeValues.Remove(key);
            }
        }
    }
}