using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NguyenManhDuc.WebApp.Models
{
    public class ProductQuantityModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedDate { get; set; }
        [ForeignKey("ProductId")]

        public ProductModel? Product { get; set; }
    }
}
