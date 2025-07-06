using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NguyenManhDuc.WebApp.Models;
using NguyenManhDuc.WebApp.Repository;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace NguyenManhDuc.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CompanyController : Controller
    {
        private readonly DataContext _dataContext;
        public CompanyController(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10;
            if (page < 1) page = 1;

            int totalItems = await _dataContext.Companies.CountAsync();
            var pager = new Paginate(totalItems, page, pageSize);

            var Companys = await _dataContext.Companies
                .OrderBy(b => b.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Pager = pager;
            return View(Companys);
        }

        public IActionResult Add() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CompanyModel CompanyModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Dữ liệu không hợp lệ.";
                return View(CompanyModel);
            }

            CompanyModel.Slug = GenerateSlug(CompanyModel.Name!);

            bool slugExists = await _dataContext.Companies
                .AnyAsync(b => b.Slug == CompanyModel.Slug);

            if (slugExists)
            {
                TempData["error"] = "Công ty đã tồn tại.";
                return View(CompanyModel);
            }

            _dataContext.Companies.Add(CompanyModel);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Thêm công ty thành công!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var Company = await _dataContext.Companies.FindAsync(id);
            if (Company == null)
            {
                TempData["error"] = "Không tìm thấy công ty.";
                return RedirectToAction("Index");
            }

            return View(Company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CompanyModel CompanyModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Dữ liệu không hợp lệ.";
                return View(CompanyModel);
            }

            CompanyModel.Slug = GenerateSlug(CompanyModel.Name!);

            bool slugExists = await _dataContext.Companies
                .AnyAsync(b => b.Id != CompanyModel.Id && b.Slug == CompanyModel.Slug);

            if (slugExists)
            {
                TempData["error"] = "Tên công ty bị trùng với công ty khác.";
                return View(CompanyModel);
            }

            _dataContext.Update(CompanyModel);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Cập nhật công ty thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var Company = await _dataContext.Companies.FindAsync(id);
            if (Company == null)
            {
                TempData["error"] = "Không tìm thấy công ty.";
                return RedirectToAction("Index");
            }

            _dataContext.Companies.Remove(Company);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Xóa công ty thành công!";
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
