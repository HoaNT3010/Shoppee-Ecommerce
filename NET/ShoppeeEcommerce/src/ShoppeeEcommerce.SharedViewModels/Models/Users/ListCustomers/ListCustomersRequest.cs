using ShoppeeEcommerce.SharedViewModels.Models.Common.Query;

namespace ShoppeeEcommerce.SharedViewModels.Models.Users.ListCustomers
{
    public record ListCustomersRequest
        : PagedRequest
    {
        public string? SearchTerm { get; set; }
    }
}
