using Microsoft.AspNetCore.Mvc;

namespace ShoppeeEcommerce.MVC.Customer.ViewComponents
{
    public class NavbarViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = new List<string> { "Electronics", "Fashion", "Home & Garden", "Gadgets" };
            return View(categories);
        }
    }
}
