using System.ComponentModel.DataAnnotations;

namespace NguyenManhDuc.WebApp.Models
{
    public class ColorModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Color { get; set; }
        public string? Code { get; set; }
        public string? Slug { get; set; }
        public ICollection<ProductColorModel> ProductColor { get; set; } = new List<ProductColorModel>();
    }
}
