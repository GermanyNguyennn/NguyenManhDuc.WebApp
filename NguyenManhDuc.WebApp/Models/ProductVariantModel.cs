using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NguyenManhDuc.WebApp.Models
{
    public class ProductVariantModel
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public ProductModel? Product { get; set; }
        public int ColorId { get; set; }
        [ForeignKey("ColorId")]
        public ColorModel? Color { get; set; }
        public int CapacityId { get; set; }
        [ForeignKey("CapacityId")]
        public CapacityModel? Capacity { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
