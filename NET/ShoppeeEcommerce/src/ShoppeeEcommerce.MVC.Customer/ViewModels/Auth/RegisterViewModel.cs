using System.ComponentModel.DataAnnotations;

namespace ShoppeeEcommerce.MVC.Customer.ViewModels.Auth
{
    public class RegisterViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required, Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
