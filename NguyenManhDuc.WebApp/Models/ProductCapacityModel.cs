using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NguyenManhDuc.WebApp.Models
{
    public class ProductCapacityModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public ProductModel? Product { get; set; }

        [Required]
        public int CapacityId { get; set; }

        [ForeignKey("CapacityId")]
        public CapacityModel? Capacity { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int Status { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}