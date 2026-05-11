using Microsoft.AspNetCore.Mvc;
using Refit;
using ShoppeeEcommerce.MVC.Customer.API;
using ShoppeeEcommerce.MVC.Customer.Utils;
using ShoppeeEcommerce.MVC.Customer.ViewModels.Orders;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.ListUserOrders;
using System.Net;

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

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            if (User.Identity?.IsAuthenticated == false)
            {
                this.SetToast("Only authenticated user can access orders page. Please login or create an account.", "error");
                return RedirectToAction("Index", "Home");
            }
            try
            {
                var order = await ordersApi.ViewOrderDetail(new PathGuidIdRequest(id.ToString()));
                return View(new OrderDetailViewModel
                {
                    Order = order
                });
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return Redirect("Error/404");
                }
            }
            this.SetToast("Something went wrong when trying to view order detail. Please try again.", "error");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("orders/cancel-modal/{id}")]
        public IActionResult CancelConfirm(Guid id)
        {
            return PartialView("Detail/_CancelOrderModal", id);
        }

        [HttpPatch]
        public async Task<IActionResult> Cancel(Guid id)
        {
            if (User.Identity?.IsAuthenticated == false)
            {
                this.SetToast("Only authenticated user can access orders page. Please login or create an account.", "error");
                return RedirectToAction("Index", "Home");
            }
            try
            {
                await ordersApi.CancelOrder(new PathGuidIdRequest(id.ToString()));

                var order = await ordersApi.ViewOrderDetail(new PathGuidIdRequest(id.ToString()));

                this.SetHTMXToast("Order cancelled successfully");

                return PartialView("Detail/_OrderHeader", new OrderDetailViewModel
                {
                    Order = order
                });
            }
            catch (ApiException ex)
            {
                this.SetHTMXToast("Something went wrong when trying to cancel order. Please try again.");
                throw;
            }
        }
    }
}
