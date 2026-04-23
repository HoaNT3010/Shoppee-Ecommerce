namespace ShoppeeEcommerce.SharedViewModels.Models.Users
{
    public record BaseCreatorResponse(
        Guid UserId,
        string UserName,
        string Email,
        string? FirstName,
        string? LastName);
}
