using Microsoft.AspNet.Mvc;

namespace HelloMvc
{
    public class HomeController
    {
        [HttpGet("/")]
        public IActionResult Index() => new ViewResult();
    }
}