using NguyenManhDuc.WebApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NguyenManhDuc.WebApp.Repository
{
    public class DataContext : IdentityDbContext<AppUserModel>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình quan hệ 1-1 giữa Product và ProductDetailPhone, ProductDetailLaptop
            modelBuilder.Entity<ProductModel>()
                .HasOne(p => p.ProductDetailLaptop)
                .WithOne(d => d.Product)
                .HasForeignKey<ProductDetailLaptopModel>(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductModel>()
                .HasOne(p => p.ProductDetailPhone)
                .WithOne(d => d.Product)
                .HasForeignKey<ProductDetailPhoneModel>(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ 1-n giữa Product và ProductQuantity
            modelBuilder.Entity<ProductModel>()
                .HasMany(p => p.ProductQuantity)
                .WithOne(q => q.Product)
                .HasForeignKey(q => q.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ 1-n giữa Product và ProductColor
            modelBuilder.Entity<ProductModel>()
                .HasMany(p => p.ProductColor)
                .WithOne(c => c.Product)
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ 1-n giữa Product và OrderDetail
            modelBuilder.Entity<ProductModel>()
                .HasMany(p => p.OrderDetail)
                .WithOne(o => o.Product)
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Tùy chỉnh nếu cần: Brand, Category, Company không cần cascade
            modelBuilder.Entity<ProductModel>()
                .HasOne(p => p.Brand)
                .WithMany()
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductModel>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductModel>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

        }

        public DbSet<BrandModel> Brands { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<CompanyModel> Companies { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderDetailModel> OrderDetails { get; set; }
        public DbSet<SliderModel> Sliders { get; set; }
        public DbSet<ContactModel> Contacts { get; set; }
        public DbSet<ProductQuantityModel> ProductQuantities { get; set; }
        public DbSet<InformationModel> Information { get; set; }
        public DbSet<CouponModel> Coupons { get; set; }
        public DbSet<MoMoModel> MoMos { get; set; }
        public DbSet<VNPayModel> VNPays { get; set; }
        public DbSet<ProductDetailPhoneModel> ProductDetailPhones { get; set; }
        public DbSet<ProductDetailLaptopModel> ProductDetailLaptops { get; set; }
        public DbSet<ProductColorModel> ProductColors { get; set; }
    }
}
