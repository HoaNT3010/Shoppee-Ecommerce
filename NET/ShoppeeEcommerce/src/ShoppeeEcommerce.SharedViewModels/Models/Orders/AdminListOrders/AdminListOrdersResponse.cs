namespace ShoppeeEcommerce.SharedViewModels.Models.Orders.AdminListOrders
{
    public record AdminListOrdersResponse(
        Guid Id,
        Guid UserId,
        string Status,
        decimal TotalPrice,
        DateTime CreatedDate,
        DateTime? UpdatedDate,
        int LineItemsCount);
}
