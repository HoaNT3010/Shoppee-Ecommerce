using Microsoft.AspNetCore.Mvc;

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
    }
}
