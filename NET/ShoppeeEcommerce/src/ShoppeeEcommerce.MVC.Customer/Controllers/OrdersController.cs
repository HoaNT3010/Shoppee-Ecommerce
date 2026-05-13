using Microsoft.AspNetCore.Mvc;
using Refit;
using ShoppeeEcommerce.MVC.Customer.API;
using ShoppeeEcommerce.MVC.Customer.Utils;
using ShoppeeEcommerce.MVC.Customer.ViewModels.Orders;
using ShoppeeEcommerce.MVC.Customer.ViewModels.Payment;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.ListUserOrders;
using ShoppeeEcommerce.SharedViewModels.Models.Payments.Stripe.Refund;
using System.Net;

namespace ShoppeeEcommerce.MVC.Customer.Controllers
{
    public class OrdersController(
        IOrdersApi ordersApi,
        IConfiguration configuration) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ListUserOrdersRequest request)
        {
            Response.Headers.Append("Vary", "HX-Request");
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
        public async Task<IActionResult> Detail(Guid id,
            bool? paid,
            bool? cancelled,
            bool? refunded)
        {
            if (User.Identity?.IsAuthenticated == false)
            {
                this.SetToast("Only authenticated user can access orders page. Please login or create an account.", "error");
                return RedirectToAction("Index", "Home");
            }
            try
            {
                var order = await ordersApi.ViewOrderDetail(new PathGuidIdRequest(id.ToString()));
                if (paid == true)
                    this.SetToast("Payment successful. Your order is now paid.", "success");

                if (cancelled == true)
                    this.SetToast("Order has been cancelled.", "warning");

                if (refunded == true)
                    this.SetToast("Order refunded successfully.", "info");
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

        [HttpPost("orders/create-payment/{orderId}")]
        public async Task<IActionResult> CreatePayment(Guid orderId)
        {
            try
            {
                var result = await ordersApi.CreatePayment(new PathGuidIdRequest(orderId.ToString()));
                this.SetToast("Request payment for order successful. Please complete the order payment.");
                Response.Headers["HX-Redirect"] = Url.Action("Detail", new { id = orderId });
                return Ok();
            }
            catch (ApiException ex)
            {
                this.SetHTMXToast("Failed to create payment for order. Please try again.", "error");
                return NoContent();
            }
        }

        [HttpPost("orders/refund")]
        public async Task<IActionResult> RefundOrderPayment(Guid orderId, Guid paymentId, decimal amount)
        {
            try
            {
                await ordersApi.RefundOrder(orderId, paymentId, new RefundAmountRequest { Amount = amount });
                this.SetHTMXToast("Refund request has been created successful and will be processed shortly.");
                return Ok();
            }
            catch (Exception ex)
            {
                this.SetHTMXToast("Failed to refund order. Please try again.", "error");
                return NoContent();
            }
        }
    }
}
