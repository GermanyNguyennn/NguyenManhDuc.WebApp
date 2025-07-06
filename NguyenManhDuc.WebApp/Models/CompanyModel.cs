using System.ComponentModel.DataAnnotations;

namespace NguyenManhDuc.WebApp.Models
{
    public class CompanyModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        public string Slug { get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ICollection<ProductModel> Product { get; set; } = new List<ProductModel>();
    }
}
