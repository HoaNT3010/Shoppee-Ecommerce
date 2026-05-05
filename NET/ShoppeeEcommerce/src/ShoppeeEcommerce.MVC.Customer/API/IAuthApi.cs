using ErrorOr;
using Refit;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.CustomerRegister;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.GetAccountInfo;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.Login;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.Logout;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.Refresh;

namespace ShoppeeEcommerce.MVC.Customer.API
{
    public interface IAuthApi
    {
        [Post("/auth/login")]
        Task<LoginResponse> Login([Body] LoginRequest request);
        [Post("/auth/logout")]
        Task Logout([Body] LogoutRequest request);
        [Post("/auth/refresh")]
        Task<RefreshResponse> Refresh([Body] RefreshRequest request);
        [Post("/customers/register")]
        Task RegisterCustomer([Body] CustomerRegisterRequest request);
        [Get("/auth/info")]
        Task<GetAccountInfoResponse> GetAccountInfo();
    }
}
