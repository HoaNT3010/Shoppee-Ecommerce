using Refit;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Products;

namespace ShoppeeEcommerce.MVC.Customer.API
{
    public interface IProductsApi
    {
        [Get("/products/{request.id}")]
        Task<BaseProductResponse> GetById(PathGuidIdRequest request);
    }
}
