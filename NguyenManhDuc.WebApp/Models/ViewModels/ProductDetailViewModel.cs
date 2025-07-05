namespace NguyenManhDuc.WebApp.Models.ViewModels
{
    public class ProductDetailViewModel
    {
        public ProductModel Product { get; set; }
        public ProductDetailPhoneModel? PhoneDetail { get; set; }
        public ProductDetailLaptopModel? LaptopDetail { get; set; }

        public CategoryModel Category => Product?.Category!;
        public BrandModel Brand => Product?.Brand!;
        public CompanyModel Company => Product?.Company!;

        public List<ProductColorModel>? Colors { get; set; }
        public List<ProductCapacityModel>? Capacities { get; set; }
    }
}
