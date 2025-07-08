using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenManhDuc.WebApp.Models;
using NguyenManhDuc.WebApp.Repository;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

namespace NguyenManhDuc.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class VersionController : Controller
    {
        private readonly DataContext _dataContext;

        public VersionController(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10;
            if (page < 1) page = 1;

            int totalItems = await _dataContext.Versions.CountAsync();
            var pager = new Paginate(totalItems, page, pageSize);

            var capacities = await _dataContext.Versions
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Pager = pager;
            return View(capacities);
        }

        public IActionResult Add() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(VersionModel capacityModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Invalid data.";
                return View(capacityModel);
            }

            capacityModel.Slug = GenerateSlug(capacityModel.Version!);

            bool slugExists = await _dataContext.Versions
                .AnyAsync(c => c.Slug == capacityModel.Slug);

            if (slugExists)
            {
                TempData["error"] = "Version already exists.";
                return View(capacityModel);
            }

            _dataContext.Versions.Add(capacityModel);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Version added successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var capacities = await _dataContext.Versions.FindAsync(id);
            if (capacities == null)
            {
                TempData["error"] = "Version not found.";
                return RedirectToAction("Index");
            }

            return View(capacities);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VersionModel capacityModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Invalid data.";
                return View(capacityModel);
            }

            capacityModel.Slug = GenerateSlug(capacityModel.Version!);

            bool slugExists = await _dataContext.Versions
                .AnyAsync(c => c.Id != capacityModel.Id && c.Slug == capacityModel.Slug);

            if (slugExists)
            {
                TempData["error"] = "The version name is duplicated with another version.";
                return View(capacityModel);
            }

            _dataContext.Versions.Update(capacityModel);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Version updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var capacities = await _dataContext.Versions.FindAsync(id);
            if (capacities == null)
            {
                TempData["error"] = "Version not found.";
                return RedirectToAction("Index");
            }

            _dataContext.Versions.Remove(capacities);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Version deleted successfully!";
            return RedirectToAction("Index");
        }

        private string GenerateSlug(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "";

            // Bước 0: Thay thế các ký tự đặc biệt tiếng Việt như Đ/đ
            name = name.Replace("Đ", "D").Replace("đ", "d");

            // Bước 1: Chuẩn hóa Unicode (loại bỏ dấu tiếng Việt)
            string normalized = name.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            string slug = sb.ToString().Normalize(NormalizationForm.FormC);

            // Bước 2: Chuyển sang chữ thường và loại bỏ ký tự đặc biệt
            slug = slug.ToLowerInvariant();
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");      // chỉ giữ lại chữ, số, khoảng trắng và -
            slug = Regex.Replace(slug, @"\s+", "-");              // thay khoảng trắng bằng dấu gạch ngang
            slug = Regex.Replace(slug, @"-+", "-");               // gộp nhiều dấu - liền nhau thành 1

            return slug.Trim('-'); // loại bỏ dấu - ở đầu/cuối
        }
    }
}
