using Refit;
using ShoppeeEcommerce.SharedViewModels.Models.Categories;

namespace ShoppeeEcommerce.MVC.Customer.API
{
    public interface ICategoriesApi
    {
        [Get("/categories")]
        Task<List<BaseCategoryResponse>> GetActiveCategories();
    }
}
