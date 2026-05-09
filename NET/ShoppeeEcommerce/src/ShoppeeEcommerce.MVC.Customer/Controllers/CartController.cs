using Microsoft.AspNetCore.Mvc;
using Refit;
using ShoppeeEcommerce.MVC.Customer.API;
using ShoppeeEcommerce.MVC.Customer.Utils;
using ShoppeeEcommerce.SharedViewModels.Models.Carts.AddItem;

namespace ShoppeeEcommerce.MVC.Customer.Controllers
{
    public class CartController(
        ICartApi cartApi)
        : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

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

                throw;
            }
        }
    }
}
