using System.ComponentModel.DataAnnotations;

namespace NguyenManhDuc.WebApp.Models
{
    public class CapacityModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Capacity { get; set; }
        public string Slug { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ICollection<ProductCapacityModel> ProductCapacity { get; set; } = new List<ProductCapacityModel>();
    }
}
