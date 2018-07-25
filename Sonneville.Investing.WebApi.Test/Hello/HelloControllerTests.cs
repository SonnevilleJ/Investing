using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Sonneville.Investing.WebApi.Hello;

namespace Sonneville.Investing.WebApi.Test.Hello
{
    [TestFixture]
    public class HelloControllerTests : ControllerTestsBase<HelloController>
    {
        [Test]
        public override void ControllerShouldHaveRouteTemplate()
        {
            AssertRouteTemplate("api/hello");
        }

        [Test]
        public void GetExample()
        {
            AssertGetMethod(Controller.Hello);
        }

        [Test]
        public void PostExample()
        {
            AssertPostMethod<string, CreatedAtRouteResult>(Controller.HelloPost);
        }

        protected override HelloController InstantiateController()
        {
            return new HelloController();
        }
    }
}