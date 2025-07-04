using System.ComponentModel.DataAnnotations;

namespace NguyenManhDuc.WebApp.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Slug { get; set; }
        public int Status { get; set; }
        public ICollection<ProductModel> Product { get; set; } = new List<ProductModel>();
    }
}
