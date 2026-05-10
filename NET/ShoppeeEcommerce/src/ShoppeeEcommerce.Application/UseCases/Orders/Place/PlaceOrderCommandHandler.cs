using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.Abstractions.Services;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Enums;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.UseCases.Orders.Place
{
    internal class PlaceOrderCommandHandler(
        IRepository<Order, Guid> orderRepo,
        IRepository<Product, Guid> productRepo,
        ICartService cartService,
        IUnitOfWork uow,
        ILogger<PlaceOrderCommandHandler> logger)
        : IRequestHandler<PlaceOrderCommand, ErrorOr<Created>>
    {
        public async Task<ErrorOr<Created>> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
        {
            // Order will be created based on the existing items in the cart
            // Required authenticated user with shopping cart that has valid items.

            // Fetch cart
            var cart = await cartService.GetUserCart(request.UserId, cancellationToken);
            if (cart is null) return Errors.OrderErrors.NoCartInfo();
            if (cart.IsGuestCart) return Errors.OrderErrors.GuestCartNotAllowed();
            if (cart.CartItems.Count == 0) return Errors.OrderErrors.EmptyCart();

            var products = await productRepo.ListAsync(new ProductsByIdsSpec(cart.CartItems.Select(i => i.ProductId)));
            // Check total product count match with item count
            if (cart.CartItems.Count != products.Count) return Errors.OrderErrors.ItemCountNotMatch();
            // Check invalid products
            if (products.Any(p => p.IsDeleted || p.Status != ProductStatus.Published)) return Errors.OrderErrors.ContainsInvalidProducts();

            var order = Order.CreateNewOrder(request.UserId);
            foreach (var item in cart.CartItems)
            {
                var product = products.First(p => p.Id == item.ProductId);
                order.Items.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductSKU = product.SKU,
                    ProductImgUrl = product.MainImage?.Url,
                    PriceSnapshot = product.Price,
                    Quantity = item.Quantity,
                });
            }
            order.RecalculateTotal();
            try
            {
                await orderRepo.AddAsync(order, cancellationToken);
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred when trying to create new order: {ExMsg}", ex.Message);
                return Errors.OrderErrors.CreateOrderFailed();
            }

            // Side effect: Empty cart
            try
            {
                cart.Clear();
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch
            {
            }

            return Result.Created;
        }
    }
}
