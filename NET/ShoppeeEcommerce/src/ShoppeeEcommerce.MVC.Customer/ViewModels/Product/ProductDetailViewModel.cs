namespace ShoppeeEcommerce.MVC.Customer.ViewModels.Product
{
    public class ProductDetailViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        //public int SelectedImageId { get; set; }
        public List<ProductImageViewModel> Images { get; set; }
        public List<ProductCategoryViewModel> Categories { get; set; }
    }
}
