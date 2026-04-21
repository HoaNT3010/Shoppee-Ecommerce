namespace ShoppeeEcommerce.SharedViewModels.Models.Authentication.Refresh
{
    public record RefreshResponse(
        string AccessToken,
        string RefreshToken);
}
