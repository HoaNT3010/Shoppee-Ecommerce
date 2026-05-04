using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Models.Users.ListCustomers;
using ShoppeeEcommerce.WebAPI.Common.Validators.Query;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Users.ListCustomers
{
    public class ListCustomersRequestValidator
        : PagedRequestValidator<ListCustomersRequest>
    {
        public ListCustomersRequestValidator()
        {
            RuleFor(x => x.SearchTerm)
                .MaximumLength(100)
                .WithMessage("Search term must not exceed 100 characters.")
                .When(x => x.SearchTerm is not null);
        }
    }
}
