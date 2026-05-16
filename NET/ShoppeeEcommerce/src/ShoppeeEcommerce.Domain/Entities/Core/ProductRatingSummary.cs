using ShoppeeEcommerce.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppeeEcommerce.Domain.Entities.Core
{
    public sealed class ProductRatingSummary
        : BaseEntity<Guid>
    {
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        public int TotalCount { get; set; }
        public decimal TotalSum { get; set; }

        public int Count1Star { get; set; }
        public int Count2Star { get; set; }
        public int Count3Star { get; set; }
        public int Count4Star { get; set; }
        public int Count5Star { get; set; }

        [NotMapped]
        public decimal AverageRating => TotalCount == 0 ? 0 : TotalSum / TotalCount;

        [NotMapped]
        public Dictionary<int, int> RatingDistribution => new()
        {
            [1] = Count1Star,
            [2] = Count2Star,
            [3] = Count3Star,
            [4] = Count4Star,
            [5] = Count5Star,
        };

        [NotMapped]
        public Dictionary<int, decimal> DistributionPercentage
            => RatingDistribution
            .ToDictionary(
                k => k.Key,
                k => TotalCount == 0 ? 0 : Math.Round((decimal)k.Value / TotalCount * 100, 1)
            );
    }
}
