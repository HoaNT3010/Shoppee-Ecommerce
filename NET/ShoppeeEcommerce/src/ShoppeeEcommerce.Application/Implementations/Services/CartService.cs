using Microsoft.Extensions.Logging;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.Abstractions.Services;
using ShoppeeEcommerce.Application.UseCases.Carts.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Application.Implementations.Services
{
    internal class CartService(
        IRepository<Cart, Guid> cartRepo,
        ICartOwnerProvider ownerProvider,
        IUnitOfWork uow,
        ILogger<CartService> logger,
        IRepository<Product, Guid> productRepo) : ICartService
    {
        public async Task AddItemAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
        {
            var cart = await GetOrCreateCartAsync(cancellationToken);
            var product = await productRepo.FirstOrDefaultAsync(new PublicProductByIdSpec(productId, false, false), cancellationToken);
            if (product == null) throw new ArgumentNullException(nameof(product), $"Failed to find the corresponding product with ID '{productId}' to add into cart.");
            try
            {
                cart.AddItem(productId, quantity, product.Price);
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when trying to add new product to cart: {ExMsg}", ex.Message);
                throw;
            }
        }

        public async Task ClearAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var cart = await GetOrCreateCartAsync(cancellationToken);
                if (cart.CartItems.Count == 0) return;
                cart.Clear();
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when trying to clear cart: {ExMsg}", ex.Message);
                throw;
            }
        }

        public async Task<Cart> GetCartAsync(CancellationToken cancellationToken = default)
            => await GetOrCreateCartAsync(cancellationToken);

        public async Task<Cart> GetOrCreateCartAsync(CancellationToken cancellationToken = default)
        {
            var (userId, sessionId) = await ownerProvider.GetOwnerAsync(cancellationToken);

            var userCart = userId == null
                ? null
                : await cartRepo.FirstOrDefaultAsync(new CartWithItemsSpec(userId), cancellationToken);
            var guestCart = sessionId == null
                ? null
                : await cartRepo.FirstOrDefaultAsync(new CartWithItemsSpec(sessionId: sessionId), cancellationToken);

            // Both carts exist -> Merge guest to user and delete guest
            if (userCart != null && guestCart != null)
            {
                try
                {
                    userCart.Merge(guestCart);
                    cartRepo.Delete(guestCart);
                    await uow.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error when trying to merge user cart and delete guest cart (UserId: '{UserId}' - SessionId: {SessionId}): {ExMsg}", userId, sessionId, ex.Message);
                    throw;
                }
                return userCart;
            }
            // only guest cart exists but user is authenticated -> Map UserId to guest cart
            if (userCart == null && guestCart != null && userId != null)
            {
                try
                {
                    guestCart.MapToUser(userId.Value);
                    await uow.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error when trying to map UserId to guest cart (UserId: '{UserId}' - SessionId: {SessionId}): {ExMsg}", userId, sessionId, ex.Message);
                    throw;
                }
                return guestCart;
            }
            // Normal cases where only 1 cart exists
            if (userCart != null) return userCart;
            if (guestCart != null) return guestCart;

            // No cart exists -> Create new cart
            var cart = new Cart
            {
                UserId = userId,
                SessionId = sessionId
            };
            try
            {
                await cartRepo.AddAsync(cart, cancellationToken);
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when trying to create new cart (UserId: '{UserId}' - SessionId: {SessionId}): {ExMsg}", userId, sessionId, ex.Message);
                throw;
            }
            return cart;
        }

        public async Task RemoveItemAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            try
            {
                var cart = await GetOrCreateCartAsync(cancellationToken);
                cart.RemoveItem(productId);
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when trying to remove product with ID '{ProductId}' from cart: {ExMsg}", productId, ex.Message);
                throw;
            }
        }

        public async Task UpdateQuantityAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
        {
            try
            {
                var cart = await GetOrCreateCartAsync(cancellationToken);
                cart.UpdateQuantity(productId, quantity);
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when trying to update quantity of product with ID '{ProductId}' in cart: {ExMsg}", productId, ex.Message);
                throw;
            }
        }
    }
}
