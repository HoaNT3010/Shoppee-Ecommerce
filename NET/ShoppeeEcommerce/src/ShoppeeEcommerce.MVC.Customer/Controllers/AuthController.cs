using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Refit;
using ShoppeeEcommerce.MVC.Customer.API;
using ShoppeeEcommerce.MVC.Customer.Utils;
using ShoppeeEcommerce.MVC.Customer.ViewModels.Auth;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.CustomerRegister;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.Login;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.Logout;

namespace ShoppeeEcommerce.MVC.Customer.Controllers
{
    public class AuthController(
        IAuthApi authApi) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userName = User.FindFirst("name")?.Value;
                this.SetToast($"Welcome back, {userName ?? "customer"}.", "info");
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> HandleLogin(
            LoginViewModel model,
            string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return PartialView("_LoginForm", model);
            try
            {
                var response = await authApi.Login(new LoginRequest(
                    model.Email,
                    model.Password
                ));
                await AuthCookieHelper.SetAuthCookie(
                    HttpContext,
                    response.AccessToken,
                    response.RefreshToken,
                    model.RememberMe);

                this.SetToast("Login successful.");
                Response.Headers["HX-Redirect"] = Url.Action("Index", "Home");
                return Ok();
            }
            catch (ApiException ex)
            {
                await ex.ToModelState(ModelState);
                return PartialView("_LoginForm", model);
            }
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> HandleRegister(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_RegisterForm", model);
            try
            {
                await authApi.RegisterCustomer(new CustomerRegisterRequest(
                    model.UserName,
                    model.Email,
                    model.Password,
                    model.ConfirmPassword,
                    model.FirstName,
                    model.LastName
                ));
                this.SetToast("Registration successful. Please login.");
                Response.Headers["HX-Redirect"] = Url.Action("Login");
                return Ok();
            }
            catch (ApiException ex)
            {
                await ex.ToModelState(ModelState);
                return PartialView("_RegisterForm", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string? returnUrl = null)
        {
            var refreshToken = User.FindFirst(AuthCookieHelper.RefreshTokenClaimName)?.Value;
            if (refreshToken != null) await authApi.Logout(new LogoutRequest(refreshToken));
            await HttpContext.SignOutAsync();

            this.SetToast("Logout successful.");
            return Redirect(returnUrl ?? "/");
        }

        [HttpGet]
        public IActionResult ForgotPassword(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userName = User.FindFirst("name")?.Value;
                this.SetToast($"Welcome back, {userName ?? "customer"}.", "info");
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}
