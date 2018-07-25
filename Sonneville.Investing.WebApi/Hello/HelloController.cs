using Microsoft.AspNetCore.Mvc;

namespace Sonneville.Investing.WebApi.Hello
{
    [Route("api/hello")]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public string Hello()
        {
            return "Hello World!";
        }

        [HttpPost]
        public CreatedAtRouteResult HelloPost([FromBody] string input)
        {
            return CreatedAtRoute("blah", "result");
        }
    }
}