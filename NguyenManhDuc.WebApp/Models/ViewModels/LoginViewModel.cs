using System.ComponentModel.DataAnnotations;

namespace NguyenManhDuc.WebApp.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public string? ReturnURL { get; set; }
    }
}
