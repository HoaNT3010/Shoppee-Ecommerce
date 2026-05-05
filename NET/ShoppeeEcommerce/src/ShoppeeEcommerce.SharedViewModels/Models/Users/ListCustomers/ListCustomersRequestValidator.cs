using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Models.Common.Query;

namespace ShoppeeEcommerce.SharedViewModels.Models.Users.ListCustomers
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
