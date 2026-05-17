using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;
using ShoppeeEcommerce.SharedViewModels.Models.Products.ViewRatingSummary;

namespace ShoppeeEcommerce.Application.UseCases.Products.ViewRatingSummary
{
    internal class ViewProductRatingSummaryQueryHandler(
        IRepository<Product, Guid> repo)
        : IRequestHandler<ViewProductRatingSummaryQuery, ErrorOr<ViewProductRatingSummaryResponse?>>
    {
        public async Task<ErrorOr<ViewProductRatingSummaryResponse?>> Handle(
            ViewProductRatingSummaryQuery request,
            CancellationToken cancellationToken)
        {
            var product = await repo.FirstOrDefaultAsync(new ViewProductRatingSummarySpec(request.ProductId), cancellationToken);
            if (product is null) return Errors.ProductErrors.ProductNotFoundWithId(request.ProductId.ToString());
            // Only return null/empty result, not return error
            // Since a product with no summary is a proper state.
            if (product.RatingSummary is null) return (ViewProductRatingSummaryResponse?)null;

            return new ViewProductRatingSummaryResponse
            {
                ProductId = product.Id,
                AverageRating = product.RatingSummary.AverageRating,
                TotalCount = product.RatingSummary.TotalCount,
                Distribution = new ProductRatingDistributionResponse
                {
                    Star1 = new ProductRatingDistributionItemResponse
                    {
                        Count = product.RatingSummary.RatingDistribution[1],
                        Percentage = product.RatingSummary.DistributionPercentage[1]
                    },
                    Star2 = new ProductRatingDistributionItemResponse
                    {
                        Count = product.RatingSummary.RatingDistribution[2],
                        Percentage = product.RatingSummary.DistributionPercentage[2]
                    },
                    Star3 = new ProductRatingDistributionItemResponse
                    {
                        Count = product.RatingSummary.RatingDistribution[3],
                        Percentage = product.RatingSummary.DistributionPercentage[3]
                    },
                    Star4 = new ProductRatingDistributionItemResponse
                    {
                        Count = product.RatingSummary.RatingDistribution[4],
                        Percentage = product.RatingSummary.DistributionPercentage[4]
                    },
                    Star5 = new ProductRatingDistributionItemResponse
                    {
                        Count = product.RatingSummary.RatingDistribution[5],
                        Percentage = product.RatingSummary.DistributionPercentage[5]
                    },
                }
            };
        }
    }
}
