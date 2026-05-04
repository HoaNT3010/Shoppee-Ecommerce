namespace ShoppeeEcommerce.SharedViewModels.Models.Users.ListCustomers
{
    public record ListCustomersResponse(
        Guid Id,
        string UserName,
        string Email,
        string? FirstName,
        string? LastName);
}
