namespace ShoppeeEcommerce.WebAPI.Domain.Abstractions
{
    public interface ITrackable
    {
        DateTime CreatedDate { get; set; }
        DateTime? UpdatedDate { get; set; }
    }
}
