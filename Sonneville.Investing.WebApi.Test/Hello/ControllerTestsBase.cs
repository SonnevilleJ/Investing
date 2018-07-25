using System;
using System.Linq;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using NUnit.Compatibility;
using NUnit.Framework;

namespace Sonneville.Investing.WebApi.Test.Hello
{
    [TestFixture]
    public abstract class ControllerTestsBase<TController> where TController : ControllerBase
    {
        [SetUp]
        public void Setup()
        {
            Controller = InstantiateController();
        }

        protected TController Controller;

        protected abstract TController InstantiateController();

        protected void AssertRouteTemplate(string expected)
        {
            var routeAttribute = Controller.GetType().GetAttribute<RouteAttribute>();

            Assert.AreEqual(expected, routeAttribute.Template);
        }

        protected void AssertGetMethod<TReturn>(Func<TReturn> method)
        {
            Assert.IsNotNull(method.Method.GetAttribute<HttpGetAttribute>());
        }

        protected void AssertPostMethod<TInput, TReturn>(Func<TInput, TReturn> method) where TReturn : IActionResult
        {
            Assert.IsNotNull(method.Method.GetAttribute<HttpPostAttribute>());

            var parameterInfo = method.Method.GetParameters().Single();
            Assert.AreEqual(typeof(FromBodyAttribute), parameterInfo.CustomAttributes.Single().AttributeType);
        }

        [Test]
        public abstract void ControllerShouldHaveRouteTemplate();
    }
}