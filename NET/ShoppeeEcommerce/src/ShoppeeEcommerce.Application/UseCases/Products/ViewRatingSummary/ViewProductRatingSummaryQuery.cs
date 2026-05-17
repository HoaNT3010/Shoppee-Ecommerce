using ErrorOr;
using MediatR;
using ShoppeeEcommerce.SharedViewModels.Models.Products.ViewRatingSummary;

namespace ShoppeeEcommerce.Application.UseCases.Products.ViewRatingSummary
{
    public record ViewProductRatingSummaryQuery(
        Guid ProductId)
        : IRequest<ErrorOr<ViewProductRatingSummaryResponse?>>;
}
