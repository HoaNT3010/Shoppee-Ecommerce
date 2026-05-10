using Microsoft.AspNetCore.Mvc;
using Refit;
using ShoppeeEcommerce.MVC.Customer.API;
using ShoppeeEcommerce.MVC.Customer.Utils;
using ShoppeeEcommerce.SharedViewModels.Models.Carts.AddItem;
using ShoppeeEcommerce.SharedViewModels.Models.Carts.UpdateQuantity;
using ShoppeeEcommerce.SharedViewModels.Models.Common;

namespace ShoppeeEcommerce.MVC.Customer.Controllers
{
    public class CartController(
        ICartApi cartApi)
        : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var cart = await cartApi.ViewCart();
                return View(cart);
            }
            catch (ApiException)
            {
                throw;
            }
        }

        [HttpPost("/cart/add-item")]
        public async Task<IActionResult> AddItem(Guid productId)
        {
            try
            {
                await cartApi.AddItem(new AddCartItemRequest
                {
                    ProductId = productId,
                    Quantity = 1,
                });

                this.SetHTMXToast("Added product to cart successfully.");
                return Ok();
            }
            catch (ApiException)
            {
                this.SetHTMXToast("Failed to add product to cart, please try again.", "error");
                throw;
            }
        }

        [HttpDelete("/cart/items/remove")]
        public async Task<IActionResult> RemoveItem(Guid productId)
        {
            try
            {
                await cartApi.RemoveItem(new PathGuidIdRequest(productId.ToString()));
                var cart = await cartApi.ViewCart();
                this.SetHTMXToast($"Remove product successfully.");
                return PartialView("_CartRefresh", cart);
            }
            catch (ApiException)
            {
                this.SetHTMXToast("Failed to remove product, please try again.", "error");
                throw;
            }
        }

        [HttpPatch("/cart/update-quantity")]
        public async Task<IActionResult> UpdateQuantity(Guid productId, int quantity)
        {
            try
            {
                await cartApi.UpdateQuantity(productId, new QuantityRequest { Quantity = quantity});
                var cart = await cartApi.ViewCart();
                this.SetHTMXToast("Update product quantity successfully.");
                return PartialView("_CartRefresh", cart);
            }
            catch (ApiException)
            {
                this.SetHTMXToast("Failed to update product quantity, please try again.", "error");
                throw;
            }
        }

        [HttpDelete("/cart")]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                await cartApi.ClearCart();
                var cart = await cartApi.ViewCart();
                this.SetHTMXToast("Clear shopping cart successfully.");
                return PartialView("_CartRefresh", cart);
            }
            catch (ApiException)
            {
                this.SetHTMXToast("Failed to clear shopping cart, please try again.", "error");
                throw;
            }
        }
    }
}
