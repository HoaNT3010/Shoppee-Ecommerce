namespace ShoppeeEcommerce.SharedViewModels.Models.Products.Create
{
    public record CreateProductResponse(
        Guid Id,
        string Name,
        string Description,
        decimal Price,
        string SKU,
        string Status,
        List<string> Categories,
        DateTime CreatedAt);
}
