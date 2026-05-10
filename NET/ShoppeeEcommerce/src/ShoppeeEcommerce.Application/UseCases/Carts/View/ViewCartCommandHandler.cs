using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.Abstractions.Services;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.SharedViewModels.Models.Carts.View;

namespace ShoppeeEcommerce.Application.UseCases.Carts.View
{
    internal class ViewCartCommandHandler(
        IRepository<Product, Guid> productRepo,
        IUnitOfWork uow,
        ICartService cartService)
        : IRequestHandler<ViewCartCommand, ErrorOr<ViewCartResponse>>
    {
        public async Task<ErrorOr<ViewCartResponse>> Handle(ViewCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await cartService.GetCartAsync(cancellationToken);
            // If cart does not have any item, return empty cart response
            if (cart.CartItems.Count == 0) return new ViewCartResponse { LastModifiedDate = cart.LastModifiedDate };

            var cartProductIds = cart.CartItems.Select(i => i.ProductId).ToHashSet();
            // Use HashSet to store products
            var products = (await productRepo.ListAsync(new ProductsByIdsSpec(cartProductIds, ignoreQueryFilter: true), cancellationToken)).ToHashSet();

            // Filtering cart item
            var removedItems = new List<Guid>();

            // Aggregating cart view
            var result = new ViewCartResponse();
            foreach (var item in cart.CartItems)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                // If item is not found, do not add item into cart view
                // store the Id in a collection to remove from cart in DB
                if (product is null)
                {
                    removedItems.Add(item.ProductId);
                }
                else
                {
                    result.Items.Add(new ViewCartItemResponse
                    {
                        Id = item.Id,
                        ProductId = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        OldPrice = item.UnitPriceSnapshot != product.Price ? item.UnitPriceSnapshot : null,
                        ImgUrl = product.MainImage?.Url,
                        Quantity = item.Quantity,
                        Status = CheckStatus(product, item)
                    });
                }
            }
            result.HasDeletedItem = removedItems.Count > 0;
            result.HasInactiveItem = result.Items.Any(i => i.Status == CartItemStatus.Inactive);
            result.HasPriceChangedItem = result.Items.Any(i => i.Status == CartItemStatus.PriceChanged);
            result.LastModifiedDate = cart.LastModifiedDate;

            // Try deleting unavailable products in cart
            // Future: can move to background service or domain event
            try
            {
                foreach (var id in removedItems)
                {
                    await cartService.RemoveItemAsync(id, cancellationToken);
                }
            }
            catch
            {
            }

            return result;
        }

        private CartItemStatus CheckStatus(Product product, CartItem item)
        {
            if (!product.IsActive()) return CartItemStatus.Inactive;
            if (product.Price != item.UnitPriceSnapshot) return CartItemStatus.PriceChanged;
            return CartItemStatus.Available;
        }
    }
}
