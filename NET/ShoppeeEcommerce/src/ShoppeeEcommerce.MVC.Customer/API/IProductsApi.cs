using Refit;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Products;
using ShoppeeEcommerce.SharedViewModels.Models.Products.GetFeatured;
using ShoppeeEcommerce.SharedViewModels.Models.Products.GetNewest;

namespace ShoppeeEcommerce.MVC.Customer.API
{
    public interface IProductsApi
    {
        [Get("/products/{request.id}")]
        Task<BaseProductResponse> GetById(PathGuidIdRequest request);
        [Get("/products/newest")]
        Task<List<BaseProductSummaryResponse>> GetNewest([Query] GetNewestProductsRequest request);
        [Get("/products/featured")]
        Task<List<BaseProductSummaryResponse>> GetFeatured([Query] GetFeaturedProductsRequest request);
        // Destruct request object to handle CategoryIds query parameter
        // Since Refit flatten the Ids list if put in an object
        [Get("/products")]
        Task<PagedList<ListProductResponse>> ListProducts([AliasAs("searchTerm")] string? searchTerm,
            [AliasAs("minPrice")] decimal? minPrice,
            [AliasAs("maxPrice")] decimal? maxPrice,
            [Query(CollectionFormat.Multi)]
            [AliasAs("categoryIds")]
            List<string>? categoryIds,
            [AliasAs("isFeatured")] bool? isFeatured,
            [AliasAs("sortBy")] string? sortBy,
            [AliasAs("sortDesc")] bool? sortDesc,
            [AliasAs("pageIndex")] int? pageIndex,
            [AliasAs("pageSize")] int? pageSize);
    }
}
