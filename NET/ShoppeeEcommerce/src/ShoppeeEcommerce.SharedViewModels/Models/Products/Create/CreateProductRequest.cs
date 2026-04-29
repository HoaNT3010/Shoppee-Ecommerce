namespace ShoppeeEcommerce.SharedViewModels.Models.Products.Create
{
    public record CreateProductRequest(
        string Name,
        string Description,
        decimal Price,
        string SKU,
        List<string> CategoryIds);
}
