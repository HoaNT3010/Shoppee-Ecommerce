using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.WebAPI.Endpoints.Categories.GetById;

namespace ShoppeeEcommerce.WebAPI.Common.Validators
{
    public class PathGuidIdRequestValidator
        : AbstractValidator<PathGuidIdRequest>
    {
        public PathGuidIdRequestValidator()
        {
            RuleFor(x => x.Id)
                .MustBeGuid();
        }
    }
}
