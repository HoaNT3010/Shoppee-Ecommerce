using ShoppeeEcommerce.SharedViewModels.Models.Users;

namespace ShoppeeEcommerce.SharedViewModels.Models.Categories
{
    public record DetailedCategoryResponse(
        Guid Id,
        string Name,
        string Description,
        DateTime CreatedDate,
        DateTime? UpdatedDate,
        bool IsDeleted,
        DateTime? DeletedDate,
        BaseCreatorResponse? Creator);
}
