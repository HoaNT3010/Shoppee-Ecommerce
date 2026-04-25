namespace ShoppeeEcommerce.SharedViewModels.Models.Authentication.GetAccountInfo
{
    public record GetAccountInfoResponse(
        Guid Id,
        string Username,
        string Email,
        string? FirstName,
        string? LastName,
        string[] Roles);
}
