namespace ShoppeeEcommerce.SharedViewModels.Models.Authentication.Login
{
    public record LoginResponse(
        string AccessToken,
        string RefreshToken);
}
