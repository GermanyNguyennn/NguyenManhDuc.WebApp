﻿using System.ComponentModel.DataAnnotations;

namespace NguyenManhDuc.WebApp.Models
{
    public class MoMoModel
    {
        [Key]
        public int Id { get; set; }
        public string? OrderId { get; set; }
        public string? OrderInfo { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
