using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Application.Abstractions.Services
{
    internal interface ICartService
    {
        Task<Cart> GetOrCreateCartAsync(CancellationToken cancellationToken = default);
        Task<Cart> GetCartAsync(CancellationToken cancellationToken = default);

        Task AddItemAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);
        Task UpdateQuantityAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);
        Task RemoveItemAsync(Guid productId, CancellationToken cancellationToken = default);
        Task ClearAsync(CancellationToken cancellationToken = default);
    }
}
