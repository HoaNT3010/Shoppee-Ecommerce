using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.MVC.Customer.API;

namespace ShoppeeEcommerce.MVC.Customer.ViewComponents
{
    public class CategoriesMenuViewComponent(
        ICategoriesApi categoriesApi)
        : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await categoriesApi.GetActiveCategories();
            return View("~/Views/Shared/Navbar/_NavbarCategories.cshtml", categories);
        }
    }
}
