using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ShoppeeEcommerce.MVC.Customer.Common
{
    public class ForceLoginFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Items.ContainsKey("ForceLogin"))
            {
                context.Result = new RedirectToActionResult(
                    "Login", "Auth", null);
            }
        }
    }
}
