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
    public class CapacityController : Controller
    {
        private readonly DataContext _dataContext;

        public CapacityController(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10;
            if (page < 1) page = 1;

            int totalItems = await _dataContext.Capacities.CountAsync();
            var pager = new Paginate(totalItems, page, pageSize);

            var capacities = await _dataContext.Capacities
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
        public async Task<IActionResult> Add(CapacityModel capacityModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Dữ liệu không hợp lệ.";
                return View(capacityModel);
            }

            capacityModel.Slug = GenerateSlug(capacityModel.Capacity);

            bool slugExists = await _dataContext.Capacities
                .AnyAsync(c => c.Slug == capacityModel.Slug);

            if (slugExists)
            {
                TempData["error"] = "Dung lượng đã tồn tại.";
                return View(capacityModel);
            }

            _dataContext.Capacities.Add(capacityModel);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Thêm dung lượng thành công!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var capacities = await _dataContext.Capacities.FindAsync(id);
            if (capacities == null)
            {
                TempData["error"] = "Không tìm thấy dung lượng.";
                return RedirectToAction("Index");
            }

            return View(capacities);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CapacityModel capacityModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Dữ liệu không hợp lệ.";
                return View(capacityModel);
            }

            capacityModel.Slug = GenerateSlug(capacityModel.Capacity);

            bool slugExists = await _dataContext.Capacities
                .AnyAsync(c => c.Id != capacityModel.Id && c.Slug == capacityModel.Slug);

            if (slugExists)
            {
                TempData["error"] = "Tên dung lượng bị trùng với dung lượng khác.";
                return View(capacityModel);
            }

            _dataContext.Capacities.Update(capacityModel);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Cập nhật màu sắc thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var capacities = await _dataContext.Capacities.FindAsync(id);
            if (capacities == null)
            {
                TempData["error"] = "Không tìm thấy màu sắc.";
                return RedirectToAction("Index");
            }

            _dataContext.Capacities.Remove(capacities);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Xóa dung lượng thành công!";
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
