using ShoppeeEcommerce.SharedViewModels.Models.Users;

namespace ShoppeeEcommerce.SharedViewModels.Models.Products.AdminGetById
{
    public record AdminGetProductByIdResponse
        : BaseProductResponse
    {
        public string Status { get; init; } = string.Empty;
        public DateTime CreatedDate { get; init; }
        public DateTime? UpdatedDate { get; init; }
        public bool IsDeleted { get; init; }
        public DateTime? DeletedDate { get; init; }
        public BaseCreatorResponse? Creator { get; init; }
    }
}
