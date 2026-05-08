namespace ShoppeeEcommerce.Application.Abstractions.Services
{
    public interface ICartOwnerProvider
    {
        Task<(Guid? userId, string? sessionId)> GetOwnerAsync(CancellationToken cancellationToken = default);
    }
}
