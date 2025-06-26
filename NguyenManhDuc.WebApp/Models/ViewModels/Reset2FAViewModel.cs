using System.ComponentModel.DataAnnotations;

namespace NguyenManhDuc.WebApp.Models.ViewModels
{
    public class Reset2FAViewModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
