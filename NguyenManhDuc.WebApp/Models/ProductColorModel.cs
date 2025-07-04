using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using NguyenManhDuc.WebApp.Repository.Validation;

namespace NguyenManhDuc.WebApp.Models
{
    public class ProductColorModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public ProductModel? Product { get; set; }

        [Required]
        public int ColorId { get; set; }

        [ForeignKey("ColorId")]
        public ColorModel? Color { get; set; }

        public string? Image { get; set; }

        public int Quantity { get; set; }

        public int Status { get; set; }

        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }
    }
}
