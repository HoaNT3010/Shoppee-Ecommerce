namespace ShoppeeEcommerce.SharedViewModels.Models.Categories.Update
{
    public record UpdateCategoryRequest(
        string CategoryId,
        string? Name,
        string? Description);
}
