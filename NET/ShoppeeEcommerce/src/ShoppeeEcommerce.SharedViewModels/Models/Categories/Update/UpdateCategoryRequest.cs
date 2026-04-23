namespace ShoppeeEcommerce.SharedViewModels.Models.Categories.Update
{
    public record UpdateCategoryRequest(
        Guid CategoryId,
        string? Name,
        string? Description);
}
