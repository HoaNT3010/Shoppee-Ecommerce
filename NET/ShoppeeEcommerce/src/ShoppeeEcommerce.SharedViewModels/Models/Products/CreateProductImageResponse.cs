namespace ShoppeeEcommerce.SharedViewModels.Models.Products
{
    public record CreateProductImageResponse(
        int Id,
        string Url,
        int DisplayOrder,
        bool IsMain);
}
