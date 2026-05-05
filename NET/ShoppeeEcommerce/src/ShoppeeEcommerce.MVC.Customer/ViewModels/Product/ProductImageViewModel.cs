namespace ShoppeeEcommerce.MVC.Customer.ViewModels.Product
{
    public class ProductImageViewModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public int DisplayOrder { get; set; }
        public string? AltText { get; set; }
    }
}
