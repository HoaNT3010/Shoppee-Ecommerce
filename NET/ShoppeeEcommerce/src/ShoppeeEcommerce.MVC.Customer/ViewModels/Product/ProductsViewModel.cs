using ShoppeeEcommerce.SharedViewModels.Models.Categories;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Products;
using ShoppeeEcommerce.SharedViewModels.Models.Products.ListProducts;

namespace ShoppeeEcommerce.MVC.Customer.ViewModels.Product
{
    public class ProductsViewModel
    {
        public ListProductsRequest Request { get; set; } = new();
        public PagedList<ListProductResponse> Products { get; set; }
        public List<BaseCategoryResponse> Categories { get; set; } = [];
    }
}
