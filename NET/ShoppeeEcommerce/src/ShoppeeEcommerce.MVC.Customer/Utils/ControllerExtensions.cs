using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ShoppeeEcommerce.MVC.Customer.Utils
{
    public static class ControllerExtensions
    {
        public static void SetToast(this Controller controller,
            string toastMsg = "Done",
            string toastType = "success")
        {
            controller.TempData["ToastMessage"] = toastMsg;
            controller.TempData["ToastType"] = toastType;
        }

        public static void SetHTMXToast(this Controller controller,
            string message = "Done",
            string type = "success")
        {
            var payload = new
            {
                showToast = new
                {
                    message,
                    type
                }
            };
            controller.Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(payload);
        }
    }
}
