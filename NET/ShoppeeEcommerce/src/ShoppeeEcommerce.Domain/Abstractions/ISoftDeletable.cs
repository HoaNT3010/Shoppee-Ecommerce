namespace ShoppeeEcommerce.Domain.Abstractions
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedDate { get; set; }
    }
}
