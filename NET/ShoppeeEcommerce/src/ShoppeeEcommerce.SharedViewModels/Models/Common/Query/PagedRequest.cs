namespace ShoppeeEcommerce.SharedViewModels.Models.Common.Query
{
    public record PagedRequest(
        int PageIndex = 1,
        int PageSize = 10);
}
