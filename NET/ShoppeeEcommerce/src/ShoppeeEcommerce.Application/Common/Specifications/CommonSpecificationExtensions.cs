using Ardalis.Specification;
using ShoppeeEcommerce.Application.Common.Query;
using ShoppeeEcommerce.Application.Utilities;
using ShoppeeEcommerce.Domain.Abstractions;
using System.Linq.Expressions;

namespace ShoppeeEcommerce.Application.Common.Specifications
{
    internal static class CommonSpecificationExtensions
    {
        public static ISpecificationBuilder<TEntity> ApplyCommonFilters<TEntity>(
            this ISpecificationBuilder<TEntity> builder,
            DateRangesSortedPagedIncludeDeletedQuery query)
            where TEntity : class, ISoftDeletable, ITrackable
        {
            // Soft-delete
            // Since global query filter already exclude soft deleted items
            // If enable include deleted, disable global query filter.
            // Now the query will include both (deleted and active) => What we want!
            if (query.IncludeDeleted.HasValue && query.IncludeDeleted.Value)
                builder.IgnoreQueryFilters();
            // If disable include deleted, do nothing ;)

            // Date range
            if (query.FromCreatedDate.HasValue)
                builder.Where(x =>
                    // Compare at the start of the date
                    x.CreatedDate >= query.FromCreatedDate.Value.StartOfDay());
            if (query.ToCreatedDate.HasValue)
                builder.Where(x =>
                    // Compare at the last tick of the date
                    x.CreatedDate <= query.ToCreatedDate.Value.Date.EndOfDay());

            return builder;
        }

        public static ISpecificationBuilder<TEntity> ApplyPaging<TEntity>(
            this ISpecificationBuilder<TEntity> builder,
            int page = 1,
            int pageSize = 10)
            where TEntity : class
        {
            return builder
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        public static ISpecificationBuilder<TEntity> ApplySorting<TEntity>(
            this ISpecificationBuilder<TEntity> builder,
            string? sortBy,
            bool? sortDesc)
            where TEntity : class, ITrackable
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                // Default sort new to old
                return builder.OrderByDescending(x => x.CreatedDate);
            }
            // Create the expression: x => x.Field
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, sortBy);
            var lambda = Expression.Lambda<Func<TEntity, object?>>(
                Expression.Convert(property, typeof(object)),
                parameter);

            if (sortDesc.HasValue && sortDesc.Value)
            {
                builder.OrderByDescending(lambda);
            }
            // Default sort ascending
            else
            {
                builder.OrderBy(lambda);
            }

            return builder;
        }
    }
}
