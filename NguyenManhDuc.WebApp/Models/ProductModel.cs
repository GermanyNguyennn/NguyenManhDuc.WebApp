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
        public string Slug { get; set; } = string.Empty;
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [ForeignKey("CategoryId")]
        public CategoryModel? Category { get; set; }
        [ForeignKey("BrandId")]
        public BrandModel? Brand { get; set; }
        [ForeignKey("CompanyId")]
        public CompanyModel? Company { get; set; }

        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }

        public ProductDetailPhoneModel? ProductDetailPhones { get; set; }
        public ProductDetailLaptopModel? ProductDetailLaptops { get; set; }
        public ICollection<ProductColorModel> ProductColors { get; set; } = new List<ProductColorModel>();
        public ICollection<ProductCapacityModel> ProductCapacities { get; set; } = new List<ProductCapacityModel>();
        public ICollection<ProductVariantModel> ProductVariants { get; set; } = new List<ProductVariantModel>();
        public ICollection<OrderDetailModel> OrderDetails { get; set; } = new List<OrderDetailModel>();

    }
}
