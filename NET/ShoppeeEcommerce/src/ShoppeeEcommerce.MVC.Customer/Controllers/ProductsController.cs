using Microsoft.AspNetCore.Mvc;
using Refit;
using ShoppeeEcommerce.MVC.Customer.API;
using ShoppeeEcommerce.MVC.Customer.Utils;
using ShoppeeEcommerce.MVC.Customer.ViewModels.Product;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Products.ListProducts;

namespace ShoppeeEcommerce.MVC.Customer.Controllers
{
    public class ProductsController(
        IProductsApi productsApi,
        ICategoriesApi categoriesApi) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ListProductsRequest request)
        {
            request.PageSize = 8;
            // Tell the browser: "The response varies based on the HX-Request header"
            Response.Headers.Append("Vary", "HX-Request");
            // HTMX swap — only the results partial is needed, skip categories
            if (Request.Headers.ContainsKey("HX-Request") && !Request.Headers.ContainsKey("HX-History-Restore-Request"))
            {
                var products = await productsApi.ListProducts(request.SearchTerm,
                    request.MinPrice,
                    request.MaxPrice,
                    request.CategoryIds,
                    request.IsFeatured,
                    request.SortBy,
                    request.SortDesc,
                    request.PageIndex,
                    request.PageSize);

                return PartialView("_ProductResults", new ProductsViewModel
                {
                    Request = request,
                    Products = products
                });
            }

            // Full page — fetch products and categories in parallel
            var productsTask = productsApi.ListProducts(request.SearchTerm,
                request.MinPrice,
                request.MaxPrice,
                request.CategoryIds,
                request.IsFeatured,
                request.SortBy,
                request.SortDesc,
                request.PageIndex,
                request.PageSize);
            var categoriesTask = categoriesApi.GetActiveCategories();
            await Task.WhenAll(productsTask, categoriesTask);

            return View(new ProductsViewModel
            {
                Request = request,
                Products = await productsTask,
                Categories = await categoriesTask
            });
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
        public async Task<IActionResult> Related(Guid id)
        {
            try
            {
                var product = await productsApi.GetById(new PathGuidIdRequest(id.ToString()));
                var related = await productsApi.ListProducts(
                    null, null, null,
                    product.Categories.Select(c => c.Id.ToString()).ToList(),
                    null, null, null, 1, 8);
                return PartialView(
                    "~/Views/Shared/Products/_ProductSlider.cshtml",
                    related.Items);
            }
            catch (ApiException ex)
            {

                throw;
            }
        }
    }
}
