using System;
using System.Linq;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
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

        protected void AssertGetMethod<TReturn>(
            Func<TReturn> method,
            Action<Func<TReturn>> verifier
        )
        {
            Assert.IsNotNull(method.Method.GetAttribute<HttpGetAttribute>());
            verifier(method);
        }

        protected void AssertPostMethod<TInput, TReturn>(
            Func<TInput, TReturn> method,
            Action<Func<TInput, TReturn>> verifier
        )
        {
            Assert.IsNotNull(method.Method.GetAttribute<HttpPostAttribute>());

            var parameterInfo = method.Method.GetParameters().Single();
            Assert.AreEqual(typeof(FromBodyAttribute), parameterInfo.CustomAttributes.Single().AttributeType);

            verifier(method);
        }

        [Test]
        public abstract void ControllerShouldHaveRouteTemplate();
    }
}