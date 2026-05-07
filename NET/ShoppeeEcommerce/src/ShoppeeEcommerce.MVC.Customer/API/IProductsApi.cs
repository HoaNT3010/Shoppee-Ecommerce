using Refit;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Products;
using ShoppeeEcommerce.SharedViewModels.Models.Products.GetFeatured;
using ShoppeeEcommerce.SharedViewModels.Models.Products.GetNewest;
using ShoppeeEcommerce.SharedViewModels.Models.Products.ListProducts;

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
        [Get("/products")]
        Task<List<ListProductResponse>> ListProducts([Query] ListProductsRequest request);
    }
}
