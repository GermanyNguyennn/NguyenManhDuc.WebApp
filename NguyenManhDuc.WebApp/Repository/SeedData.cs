﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NguyenManhDuc.WebApp.Models;

namespace NguyenManhDuc.WebApp.Repository
{
    public class SeedData
    {
        public static async Task SeedingDataAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<DataContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUserModel>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            context.Database.Migrate();

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("Customer"))
            {
                await roleManager.CreateAsync(new IdentityRole("Customer"));
            }

            var adminEmail = "manhducnguyen23092003@gmail.com";
            var adminPhone = "0964429403";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new AppUserModel
                {
                    UserName = "Admin",
                    Email = adminEmail,
                    PhoneNumber = adminPhone,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "23092003");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            if (!context.Products.Any())
            {
                // Seed Companies
                var apple = new CompanyModel { Name = "Apple", Description = "", Slug = "apple", Status = 1 };
                var microsoft = new CompanyModel { Name = "Microsoft", Description = "", Slug = "microsoft", Status = 1 };
                context.Companies.AddRange(apple, microsoft);

                // Seed Brands
                var iphone = new BrandModel { Name = "Iphone", Description = "", Slug = "iphone", Status = 1 };
                var mac = new BrandModel { Name = "Mac", Description = "", Slug = "mac", Status = 1 };
                var windows = new BrandModel { Name = "Windows", Description = "", Slug = "windows", Status = 1 };
                context.Brands.AddRange(iphone, mac, windows);

                // Seed Categories
                var phone = new CategoryModel { Name = "Phone", Description = "", Slug = "phone", Status = 1 };
                var laptop = new CategoryModel { Name = "Laptop", Description = "", Slug = "laptop", Status = 1 };
                context.Categories.AddRange(phone, laptop);

                // Save changes để EF sinh ID
                await context.SaveChangesAsync();

                // Seed Products (dùng object đã lưu ở trên, lúc này đã có Id)
                context.Products.AddRange(
                    new ProductModel { Name = "MacBook Air M4 15 inch 2025 10CPU 10GPU 24GB 512GB Sạc 70W", Image = "macbook-air.jpg", Description = "", Price = 42490000, Slug = "", BrandId = mac.Id, CategoryId = laptop.Id, CompanyId = apple.Id, Quantity = 100 },
                    new ProductModel { Name = "MacBook Pro 16 M4 Max 16CPU 40GPU 64GB 2TB", Image = "macbook-pro.jpg", Description = "", Price = 117490000, Slug = "", BrandId = mac.Id, CategoryId = laptop.Id, CompanyId = apple.Id, Quantity = 100 },

                    new ProductModel { Name = "iPhone 16 Pro Max 1TB | Chính hãng VN/A", Image = "iphone-16-pro-max.jpg", Description = "", Price = 42690000, Slug = "", BrandId = iphone.Id, CategoryId = phone.Id, CompanyId = apple.Id, Quantity = 100 },
                    new ProductModel { Name = "iPhone 16 Pro 1TB | Chính hãng VN/A", Image = "iphone-16-pro.jpg", Description = "", Price = 38990000, Slug = "", BrandId = iphone.Id, CategoryId = phone.Id, CompanyId = apple.Id, Quantity = 100 },
                    new ProductModel { Name = "iPhone 16 Plus 512GB | Chính hãng VN/A", Image = "iphone-16-plus.jpg", Description = "", Price = 38990000, Slug = "", BrandId = iphone.Id, CategoryId = phone.Id, CompanyId = apple.Id, Quantity = 100 },
                    new ProductModel { Name = "iPhone 16 512GB | Chính hãng VN/A", Image = "iphone-16.jpg", Description = "", Price = 38990000, Slug = "", BrandId = iphone.Id, CategoryId = phone.Id, CompanyId = apple.Id, Quantity = 100 },
                    new ProductModel { Name = "iPhone 16e 512GB | Chính hãng VN/A", Image = "iphone-16e.jpg", Description = "", Price = 25490000, Slug = "", BrandId = iphone.Id, CategoryId = phone.Id, CompanyId = apple.Id, Quantity = 100 }
                );

                await context.SaveChangesAsync();
            }

            if (!context.Contacts.Any())
            {
                ContactModel contactModel = new ContactModel
                {
                    Name = "Nguyễn Mạnh Đức",
                    Map = "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3723.883773346561!2d105.85400031440625!3d21.00500009317313!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3135ab4000000001%3A0x0000000000000001!2sNgõ%2084%20Phố%208%2F3%2C%20Quỳnh%20Mai%2C%20Hai%20Bà%20Trưng%2C%20Hà%20Nội!5e0!3m2!1svi!2s!4v1680000000000",
                    Email = "manhducnguyen23092003@gmail.com",
                    Phone = "0964429403"
                };

                context.Contacts.Add(contactModel);
                await context.SaveChangesAsync();
            }
        }
    }
}
