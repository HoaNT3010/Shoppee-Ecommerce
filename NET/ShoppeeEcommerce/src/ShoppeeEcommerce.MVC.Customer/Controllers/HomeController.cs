using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.MVC.Customer.API;
using ShoppeeEcommerce.MVC.Customer.Models;
using ShoppeeEcommerce.MVC.Customer.ViewModels.Home;
using ShoppeeEcommerce.SharedViewModels.Models.Products.GetFeatured;
using ShoppeeEcommerce.SharedViewModels.Models.Products.GetNewest;
using System.Diagnostics;

namespace ShoppeeEcommerce.MVC.Customer.Controllers
{
    public class HomeController(
        IProductsApi productsApi) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var featuredTask = productsApi.GetFeatured(new GetFeaturedProductsRequest(8));
            var newestTask = productsApi.GetNewest(new GetNewestProductsRequest(8));
            await Task.WhenAll(featuredTask, newestTask);
            var vm = new HomeViewModel
            {
                FeaturedProducts = featuredTask.Result,
                NewestProducts = newestTask.Result,
            };
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
