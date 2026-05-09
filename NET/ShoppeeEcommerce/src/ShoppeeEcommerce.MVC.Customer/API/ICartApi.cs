using Refit;
using ShoppeeEcommerce.SharedViewModels.Models.Carts.AddItem;
using ShoppeeEcommerce.SharedViewModels.Models.Carts.UpdateQuantity;
using ShoppeeEcommerce.SharedViewModels.Models.Carts.View;
using ShoppeeEcommerce.SharedViewModels.Models.Common;

namespace ShoppeeEcommerce.MVC.Customer.API
{
    public interface ICartApi
    {
        [Get("/cart")]
        public Task<ViewCartResponse> ViewCart();
        [Post("/cart/items")]
        public Task AddItem([Body] AddCartItemRequest request);
        [Delete("/cart/items/{request.id}")]
        public Task RemoveItem(PathGuidIdRequest request);
        [Patch("/cart/items/{id}")]
        public Task UpdateQuantity(Guid id, [Body] QuantityRequest request);
        [Delete("/cart")]
        public Task ClearCart();
    }
}
