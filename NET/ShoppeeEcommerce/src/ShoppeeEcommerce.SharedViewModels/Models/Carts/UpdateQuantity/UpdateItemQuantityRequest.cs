using Microsoft.AspNetCore.Mvc;

namespace ShoppeeEcommerce.SharedViewModels.Models.Carts.UpdateQuantity
{
    public class UpdateItemQuantityRequest
    {
        [FromRoute(Name = "id")]
        public Guid Id { get; set; }
        [FromBody]
        public QuantityRequest Quantity { get; set; }
    }

    public class QuantityRequest
    {
        public int Quantity { get; set; }
    }
}
