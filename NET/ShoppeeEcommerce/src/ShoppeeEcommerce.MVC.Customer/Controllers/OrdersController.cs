using Microsoft.AspNetCore.Mvc;
using Refit;
using ShoppeeEcommerce.MVC.Customer.API;
using ShoppeeEcommerce.MVC.Customer.Utils;
using ShoppeeEcommerce.MVC.Customer.ViewModels.Orders;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.ListUserOrders;

namespace ShoppeeEcommerce.MVC.Customer.Controllers
{
    public class OrdersController(
        IOrdersApi ordersApi) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ListUserOrdersRequest request)
        {
            if (User.Identity?.IsAuthenticated == false)
            {
                this.SetToast("Only authenticated user can access orders page. Please login or create an account.", "error");
                return RedirectToAction("Index", "Home");
            }
            // Fixed page size at 8 items for now
            request.PageSize = 8;
            if (Request.Headers.ContainsKey("HX-Request"))
            {
                try
                {
                    var orders = await ordersApi.ListUserOrders(request);
                    return PartialView("List/_OrdersRefresh", new UserOrdersViewModel
                    {
                        Orders = orders,
                        Request = request,
                    });
                }
                catch (ApiException ex)
                {

                    throw;
                }
            }

            try
            {
                var ordersList = await ordersApi.ListUserOrders(request);
                return View(new UserOrdersViewModel
                {
                    Orders = ordersList,
                    Request = request,
                });
            }
            catch (ApiException ex)
            {

                throw;
            }
        }
    }
}
