namespace ShoppeeEcommerce.Domain.Abstractions
{
    public interface IHasKey<TKey>
    {
        TKey Id { get; set; }
    }
}
