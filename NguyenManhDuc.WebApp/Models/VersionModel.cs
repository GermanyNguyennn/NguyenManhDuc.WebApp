using System.ComponentModel.DataAnnotations;

namespace NguyenManhDuc.WebApp.Models
{
    public class VersionModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Version { get; set; }
        public string Slug { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ICollection<ProductVersionModel> ProductCapacity { get; set; } = new List<ProductVersionModel>();
    }
}
