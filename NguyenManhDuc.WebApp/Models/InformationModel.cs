using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NguyenManhDuc.WebApp.Models
{
    public class InformationModel
    {
        [Key]
        public int Id { get; set; }

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUserModel? AppUser { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
