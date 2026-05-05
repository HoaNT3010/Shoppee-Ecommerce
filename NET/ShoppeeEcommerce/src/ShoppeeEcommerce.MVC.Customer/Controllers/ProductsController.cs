using Microsoft.AspNetCore.Mvc;
using Refit;
using ShoppeeEcommerce.MVC.Customer.API;
using ShoppeeEcommerce.MVC.Customer.Utils;
using ShoppeeEcommerce.MVC.Customer.ViewModels.Product;
using ShoppeeEcommerce.SharedViewModels.Models.Common;

namespace ShoppeeEcommerce.MVC.Customer.Controllers
{
    public class ProductsController(
        IProductsApi productsApi) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            try
            {
                var product = await productsApi.GetById(new PathGuidIdRequest(id.ToString()));
                var vm = new ProductDetailViewModel
                {
                    Id = id,
                    Name = product.Name,
                    Price = product.Price,
                    Description = product.Description,
                    SKU = product.SKU,
                    Images = product.Images.Select(i => new ProductImageViewModel
                    {
                        Id = i.Id,
                        Url = i.Url,
                        IsMain = i.IsMain,
                        DisplayOrder = i.DisplayOrder,
                        AltText = i.AltText
                    }).ToList(),
                    Categories = product.Categories.Select(c => new ProductCategoryViewModel
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList(),
                };
                return View(vm);
            }
            catch (ApiException ex) when (ex.IsNotFound())
            {
                return NotFound();
            }
        }

        [HttpGet]
        public IActionResult Reviews(Guid id)
        {
            return PartialView("_Reviews");
        }

        [HttpGet]
        public IActionResult Related(Guid id)
        {
            return PartialView("_RelatedProducts");
        }
    }
}
