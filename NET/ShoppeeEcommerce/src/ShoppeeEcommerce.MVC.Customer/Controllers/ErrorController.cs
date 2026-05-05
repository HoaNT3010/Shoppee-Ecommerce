using Microsoft.AspNetCore.Mvc;

namespace ShoppeeEcommerce.MVC.Customer.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/404")]
        public IActionResult NotFoundPage()
        {
            return View("NotFound");
        }
    }
}
