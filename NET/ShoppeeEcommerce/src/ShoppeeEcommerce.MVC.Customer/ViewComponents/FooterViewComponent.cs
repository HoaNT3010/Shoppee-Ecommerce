using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.MVC.Customer.API;

namespace ShoppeeEcommerce.MVC.Customer.ViewComponents
{
    public class FooterViewComponent(
        ICategoriesApi categoriesApi)
        : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cates = await categoriesApi.GetActiveCategories();
            return View(cates);
        }
    }
}
