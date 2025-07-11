﻿using System.ComponentModel.DataAnnotations;

namespace NguyenManhDuc.WebApp.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
    }
}
