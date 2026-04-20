namespace ShoppeeEcommerce.SharedViewModels.Models.Authentication.CustomerRegister
{
    public record CustomerRegisterRequest(
        string UserName,
        string Email,
        string Password,
        string ConfirmPassword,
        string? FirstName,
        string? LastName);
}
