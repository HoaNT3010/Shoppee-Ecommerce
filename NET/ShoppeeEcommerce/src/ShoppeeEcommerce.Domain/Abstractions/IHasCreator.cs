namespace ShoppeeEcommerce.Domain.Abstractions
{
    public interface IHasCreator<TKey>
    {
        TKey CreatedBy { get; set; }
    }
}
