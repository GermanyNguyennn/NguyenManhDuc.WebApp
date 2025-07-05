using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenManhDuc.WebApp.Models;
using NguyenManhDuc.WebApp.Repository;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using System.Drawing.Imaging;

namespace NguyenManhDuc.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ColorController : Controller
    {
        private readonly DataContext _dataContext;

        public ColorController(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10;
            if (page < 1) page = 1;

            int totalItems = await _dataContext.Colors.CountAsync();
            var pager = new Paginate(totalItems, page, pageSize);

            var colors = await _dataContext.Colors
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Pager = pager;
            return View(colors);
        }

        public IActionResult Add() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ColorModel colorModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Dữ liệu không hợp lệ.";
                return View(colorModel);
            }

            colorModel.Slug = GenerateSlug(colorModel.Color);

            bool slugExists = await _dataContext.Colors
                .AnyAsync(c => c.Slug == colorModel.Slug);

            if (slugExists)
            {
                TempData["error"] = "Màu sắc đã tồn tại.";
                return View(colorModel);
            }

            _dataContext.Colors.Add(colorModel);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Thêm màu sắc thành công!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var colors = await _dataContext.Colors.FindAsync(id);
            if (colors == null)
            {
                TempData["error"] = "Không tìm thấy màu sắc.";
                return RedirectToAction("Index");
            }

            return View(colors);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ColorModel colorModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Dữ liệu không hợp lệ.";
                return View(colorModel);
            }

            colorModel.Slug = GenerateSlug(colorModel.Color);

            bool slugExists = await _dataContext.Categories
                .AnyAsync(c => c.Id != colorModel.Id && c.Slug == colorModel.Slug);

            if (slugExists)
            {
                TempData["error"] = "Tên màu sắc bị trùng với màu sắc khác.";
                return View(colorModel);
            }

            _dataContext.Colors.Update(colorModel);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Cập nhật màu sắc thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var colors = await _dataContext.Colors.FindAsync(id);
            if (colors == null)
            {
                TempData["error"] = "Không tìm thấy màu sắc.";
                return RedirectToAction("Index");
            }

            _dataContext.Colors.Remove(colors);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Xóa danh mục thành công!";
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
