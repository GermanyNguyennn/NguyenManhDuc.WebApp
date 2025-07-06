using System.ComponentModel.DataAnnotations;

namespace NguyenManhDuc.WebApp.Models
{
    public class ColorModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Color { get; set; }
        public string? Code { get; set; }
        public string Slug { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ICollection<ProductColorModel> ProductColor { get; set; } = new List<ProductColorModel>();
    }
}
