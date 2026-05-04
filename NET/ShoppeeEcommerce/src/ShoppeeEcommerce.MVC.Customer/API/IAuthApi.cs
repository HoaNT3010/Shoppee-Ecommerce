using Refit;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.Login;

namespace ShoppeeEcommerce.MVC.Customer.API
{
    public interface IAuthApi
    {
        [Post("/auth/login")]
        Task<LoginResponse> Login([Body] LoginRequest request);
    }
}
