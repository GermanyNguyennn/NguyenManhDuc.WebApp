using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using NguyenManhDuc.WebApp.Repository.Validation;

namespace NguyenManhDuc.WebApp.Models
{
    public class ProductModel
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal ImportPrice { get; set; }
        public int Quantity { get; set; }
        public int Sold { get; set; }
        public string? Slug { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public int CompanyId { get; set; }

        [ForeignKey("CategoryId")]
        public CategoryModel? Category { get; set; }
        [ForeignKey("BrandId")]
        public BrandModel? Brand { get; set; }
        [ForeignKey("CompanyId")]
        public CompanyModel? Company { get; set; }

        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }

        public ProductDetailPhoneModel? ProductDetailPhone { get; set; }
        public ProductDetailLaptopModel? ProductDetailLaptop { get; set; }
        public ICollection<ProductQuantityModel> ProductQuantity { get; set; } = new List<ProductQuantityModel>();
        public ICollection<ProductColorModel> ProductColor { get; set; } = new List<ProductColorModel>();
        public ICollection<OrderDetailModel> OrderDetail { get; set; } = new List<OrderDetailModel>();

    }
}
