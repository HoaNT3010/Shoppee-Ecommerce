using Microsoft.AspNetCore.Mvc;

namespace ShoppeeEcommerce.MVC.Customer.ViewComponents
{
    public class UserCartMenuViewComponent
        : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
