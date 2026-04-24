namespace ShoppeeEcommerce.SharedViewModels.Models.Categories.AdminListCategories
{
    public record AdminListCategoryResponse(
        Guid Id,
        string Name,
        string Description,
        DateTime CreatedDate,
        bool IsDeleted);
}
