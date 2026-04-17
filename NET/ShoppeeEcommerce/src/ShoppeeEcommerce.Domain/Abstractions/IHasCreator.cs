namespace ShoppeeEcommerce.Domain.Abstractions
{
    public interface IHasCreator<TKey>
    {
        TKey CreatorId { get; set; }
    }
}
