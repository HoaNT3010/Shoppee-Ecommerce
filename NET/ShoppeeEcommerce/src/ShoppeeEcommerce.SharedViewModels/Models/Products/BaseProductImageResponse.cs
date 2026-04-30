namespace ShoppeeEcommerce.SharedViewModels.Models.Products
{
    public record BaseProductImageResponse(
        int Id,
        string Url,
        bool IsMain,
        int DisplayOrder,
        string? AltText);
}
