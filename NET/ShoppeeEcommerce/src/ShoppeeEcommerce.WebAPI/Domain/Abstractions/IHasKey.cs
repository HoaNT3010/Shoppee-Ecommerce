namespace ShoppeeEcommerce.WebAPI.Domain.Abstractions
{
    public interface IHasKey<TKey>
    {
        TKey Id { get; set; }
    }
}
