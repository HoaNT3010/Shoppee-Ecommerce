namespace ShoppeeEcommerce.SharedViewModels.Models.Products.AdminListProducts
{
    public record AdminListProductsResponse(
        Guid Id,
        string Name,
        string Description,
        decimal Price,
        string SKU,
        DateTime CreatedDate,
        bool IsDeleted,
        string Status,
        string? ImgUrl);
}
