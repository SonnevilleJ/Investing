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
            AssertGetMethod(
                Controller.Hello,
                getMethod => Assert.AreEqual("Hello World!", getMethod())
            );
        }

        [Test]
        public void PostExample()
        {
            AssertPostMethod<string, CreatedAtRouteResult>(
                Controller.HelloPost,
                postMethod =>
                {
                    var createdAtRouteResult = postMethod("");
                    Assert.AreEqual("blah", createdAtRouteResult.RouteName);
                    Assert.AreEqual("result", createdAtRouteResult.Value);
                }
            );
        }

        protected override HelloController InstantiateController()
        {
            return new HelloController();
        }
    }
}