using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NguyenManhDuc.WebApp.Models;
using NguyenManhDuc.WebApp.Repository.Validation;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NguyenManhDuc.WebApp.Repository;
using NguyenManhDuc.WebApp.Areas.Admin.Models.ViewModels;

namespace NguyenManhDuc.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10;
            page = Math.Max(1, page);
            var count = await _dataContext.Products.CountAsync();

            var pager = new Paginate(count, page, pageSize);
            var data = await _dataContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Company)
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Pager = pager;
            return View(data);
        }

        [HttpGet]
        public IActionResult Add()
        {
            SetSelectLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ProductModel productModel)
        {
            SetSelectLists(productModel.CategoryId, productModel.BrandId, productModel.CompanyId);

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Dữ liệu không hợp lệ.";
                return View(productModel);
            }

            productModel.Slug = GenerateSlug(productModel.Name!);

            if (await _dataContext.Products.AnyAsync(p => p.Slug == productModel.Slug))
            {
                TempData["error"] = "Sản phẩm đã tồn tại.";
                return View(productModel);
            }

            if (productModel.ImageUpload != null)
                productModel.Image = await SaveImageAsync(productModel.ImageUpload);

            _dataContext.Add(productModel);

            await _dataContext.SaveChangesAsync();           

            TempData["success"] = "Thêm sản phẩm thành công.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _dataContext.Products.FindAsync(id);
            if (product == null) return NotFound();

            SetSelectLists(product.CategoryId, product.BrandId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductModel productModel)
        {
            SetSelectLists(productModel.CategoryId, productModel.BrandId, productModel.CompanyId);

            var existingProduct = await _dataContext.Products.FindAsync(id);
            if (existingProduct == null) return NotFound();

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Dữ liệu không hợp lệ.";
                return View(productModel);
            }

            existingProduct.Slug = productModel.Name!.Trim().Replace(" ", "-");

            if (productModel.ImageUpload != null)
            {
                try
                {
                    DeleteImage(existingProduct.Image!);
                    existingProduct.Image = await SaveImageAsync(productModel.ImageUpload);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Không thể cập nhật hình ảnh: " + ex.Message);
                    return View(productModel);
                }
            }

            existingProduct.Name = productModel.Name;
            existingProduct.Description = productModel.Description;
            existingProduct.Price = productModel.Price;
            existingProduct.ImportPrice = productModel.ImportPrice;
            existingProduct.Quantity = productModel.Quantity;
            existingProduct.CategoryId = productModel.CategoryId;
            existingProduct.BrandId = productModel.BrandId;
            existingProduct.CompanyId = productModel.CompanyId;

            _dataContext.Update(existingProduct);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Cập nhật sản phẩm thành công.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _dataContext.Products.FindAsync(id);
            if (product == null) return NotFound();

            DeleteImage(product.Image!);

            _dataContext.Products.Remove(product);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Xóa sản phẩm thành công.";
            return RedirectToAction("Index");
        }       

        [HttpPost]
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return RedirectToAction("Index");

            var products = await _dataContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.Name!.Contains(searchTerm))
                .ToListAsync();

            ViewBag.Keyword = searchTerm;
            return View(products);
        }      

        private void SetSelectLists(int? categoryId = null, int? brandId = null, int? companyId = null)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", categoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", brandId);
            ViewBag.Companies = new SelectList(_dataContext.Companies, "Id", "Name", companyId);
        }

        private async Task<string> SaveImageAsync(IFormFile image)
        {
            var uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
            var fileName = Guid.NewGuid() + "_" + Path.GetFileName(image.FileName);
            var filePath = Path.Combine(uploadDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return fileName;
        }

        private void DeleteImage(string imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName)) return;

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "media/products", imageName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        [HttpGet]
        public async Task<IActionResult> IndexDetail(int id)
        {
            var product = await _dataContext.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            ViewBag.Colors = product.ProductColors.ToList();
            ViewBag.Capacities = product.ProductCapacities.ToList();

            if (product.CategoryId == 1) // Phone
            {
                var detail = await _dataContext.ProductDetailPhones
                    .FirstOrDefaultAsync(x => x.ProductId == id) ?? new ProductDetailPhoneModel
                    {
                        ProductId = product.Id,
                        CategoryId = product.CategoryId,
                        BrandId = product.BrandId,
                        CompanyId = product.CompanyId
                    };

                return View("ViewDetailPhone", detail);
            }
            else if (product.CategoryId == 2) // Laptop
            {
                var detail = await _dataContext.ProductDetailLaptops
                    .FirstOrDefaultAsync(x => x.ProductId == id) ?? new ProductDetailLaptopModel
                    {
                        ProductId = product.Id,
                        CategoryId = product.CategoryId,
                        BrandId = product.BrandId,
                        CompanyId = product.CompanyId
                    };

                return View("ViewDetailLaptop", detail);
            }

            return NotFound("Danh mục không hỗ trợ hiển thị chi tiết.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDetail(IFormCollection form)
        {
            int categoryId = int.Parse(form["CategoryId"]!);

            int productId = int.Parse(form["ProductId"]!);

            if (categoryId == 1)
            {
                var detail = await _dataContext.ProductDetailPhones
                    .FirstOrDefaultAsync(x => x.ProductId == productId);

                if (detail == null)
                {
                    detail = new ProductDetailPhoneModel();
                    _dataContext.ProductDetailPhones.Add(detail);
                }

                detail.ProductId = productId;
                detail.CategoryId = categoryId;
                detail.BrandId = int.Parse(form["BrandId"]!);
                detail.CompanyId = int.Parse(form["CompanyId"]!);
                detail.ScreenSize = form["ScreenSize"]!;
                detail.DisplayTechnology = form["DisplayTechnology"]!;
                detail.RearCamera = form["RearCamera"]!;
                detail.FrontCamera = form["FrontCamera"]!;
                detail.ChipSet = form["ChipSet"]!;
                detail.NFC = form["NFC"].ToString() == "true";
                detail.RAMCapacity = form["RAMCapacity"]!;
                detail.InternalStorage = form["InternalStorage"]!;
                detail.SimCard = form["SimCard"]!;
                detail.OperatingSystem = form["OperatingSystem"]!;
                detail.DisplayResolution = form["DisplayResolution"]!;
                detail.DisplayFeatures = form["DisplayFeatures"]!;
                detail.CPUType = form["CPUType"]!;

                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Lưu chi tiết điện thoại thành công!";
            }
            else if (categoryId == 2) 
            {
                var detail = await _dataContext.ProductDetailLaptops
                    .FirstOrDefaultAsync(x => x.ProductId == productId);

                if (detail == null)
                {
                    detail = new ProductDetailLaptopModel();
                    _dataContext.ProductDetailLaptops.Add(detail);
                }

                detail.ProductId = productId;
                detail.CategoryId = categoryId;
                detail.BrandId = int.Parse(form["BrandId"]!);
                detail.CompanyId = int.Parse(form["CompanyId"]!);
                detail.GraphicsCardType = form["GraphicsCardType"]!;
                detail.RAMCapacity = form["RAMCapacity"]!;
                detail.RAMType = form["RAMType"]!;
                detail.NumberOfRAMSlots = form["NumberOfRAMSlots"]!;
                detail.HardDrive = form["HardDrive"]!;
                detail.ScreenSize = form["ScreenSize"]!;
                detail.ScreenTechnology = form["ScreenTechnology"]!;
                detail.Battery = form["Battery"]!;
                detail.OperatingSystem = form["OperatingSystem"]!;
                detail.ScreenResolution = form["ScreenResolution"]!;
                detail.CPUType = form["CPUType"]!;
                detail.Interface = form["Interface"]!;

                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Lưu chi tiết laptop thành công!";
            }
            else
            {
                return BadRequest("Danh mục không hợp lệ.");
            }

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

        public async Task<IActionResult> DetailStock(int id)
        {
            var product = await _dataContext.Products
                .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                .Include(p => p.ProductCapacities).ThenInclude(pc => pc.Capacity)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            var colorStats = await _dataContext.ProductColors
                .Where(x => x.ProductId == id)
                .Select(x => new ColorStockViewModel
                {
                    ColorName = x.Color!.Color!,
                    Quantity = x.Quantity
                }).ToListAsync();

            var capacityStats = await _dataContext.ProductCapacities
                .Where(x => x.ProductId == id)
                .Select(x => new CapacityStockViewModel
                {
                    CapacityName = x.Capacity!.Capacity!,
                    Quantity = x.Quantity
                }).ToListAsync();

            var variantStats = await _dataContext.ProductVariants
                .Where(x => x.ProductId == id)
                .Select(x => new VariantStockViewModel
                {
                    ColorName = x.Color!.Color!,
                    CapacityName = x.Capacity!.Capacity!,
                    Quantity = x.Quantity
                }).ToListAsync();

            var viewModel = new AdminProductStockViewModel
            {
                Product = product,
                Colors = colorStats,
                Capacities = capacityStats,
                Variants = variantStats
            };

            return View(viewModel);
        }

        public async Task<IActionResult> ManageColorQuantity(int id)
        {
            var colorQuantities = await _dataContext.ProductColors
                .Include(pc => pc.Color)
                .Where(pc => pc.ProductId == id)
                .ToListAsync();

            ViewBag.ProductId = id;
            return View(colorQuantities);
        }

        [HttpGet]
        public async Task<IActionResult> AddColorQuantity(int id) // id là ProductId
        {
            var product = await _dataContext.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewBag.ProductId = id;
            ViewBag.Colors = await _dataContext.Colors.ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddColorQuantity(ProductColorModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Dữ liệu không hợp lệ!";
                ViewBag.ProductId = model.ProductId;
                ViewBag.Colors = await _dataContext.Colors.ToListAsync();
                return View(model);
            }

            // ✅ Thêm đoạn này để kiểm tra khóa ngoại có tồn tại không
            var productExists = await _dataContext.Products.AnyAsync(p => p.Id == model.ProductId);
            var colorExists = await _dataContext.Colors.AnyAsync(c => c.Id == model.ColorId);

            if (!productExists || !colorExists)
            {
                TempData["error"] = "Sản phẩm hoặc màu không tồn tại!";
                ViewBag.ProductId = model.ProductId;
                ViewBag.Colors = await _dataContext.Colors.ToListAsync();
                return View(model);
            }

            // Kiểm tra trùng màu cho sản phẩm
            bool exists = await _dataContext.ProductColors
                .AnyAsync(pc => pc.ProductId == model.ProductId && pc.ColorId == model.ColorId);

            if (exists)
            {
                TempData["error"] = "Màu đã tồn tại cho sản phẩm!";
                ViewBag.ProductId = model.ProductId;
                ViewBag.Colors = await _dataContext.Colors.ToListAsync();
                return View(model);
            }

            try
            {
                model.Id = 0;
                _dataContext.ProductColors.Add(model);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Thêm số lượng màu thành công!";
                return RedirectToAction("ManageColorQuantity", new { id = model.ProductId });
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi khi lưu CSDL: " + (ex.InnerException?.Message ?? ex.Message);
                ViewBag.ProductId = model.ProductId;
                ViewBag.Colors = await _dataContext.Colors.ToListAsync();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditColorQuantity(int id)
        {
            var item = await _dataContext.ProductColors.FindAsync(id);
            if (item == null) return NotFound();

            ViewBag.Colors = await _dataContext.Colors.ToListAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditColorQuantity(ProductColorModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Colors = await _dataContext.Colors.ToListAsync();
                return View(model);
            }

            var item = await _dataContext.ProductColors.FindAsync(model.Id);
            if (item == null)
            {
                TempData["error"] = "Không tìm thấy bản ghi cần sửa.";
                return RedirectToAction("ManageColorQuantity", new { id = model.ProductId });
            }

            // Kiểm tra tính hợp lệ của ProductId và ColorId
            var productExists = await _dataContext.Products.AnyAsync(p => p.Id == model.ProductId);
            var colorExists = await _dataContext.Colors.AnyAsync(c => c.Id == model.ColorId);

            if (!productExists || !colorExists)
            {
                TempData["error"] = "Sản phẩm hoặc màu không tồn tại!";
                ViewBag.Colors = await _dataContext.Colors.ToListAsync();
                return View(model);
            }

            // Chỉ cập nhật số lượng và trạng thái (không cập nhật ColorId/ProductId vì là khóa chính logic)
            item.Quantity = model.Quantity;
            item.Status = model.Status;

            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Cập nhật số lượng màu thành công!";
            return RedirectToAction("ManageColorQuantity", new { id = item.ProductId });
        }

        public async Task<IActionResult> DeleteColorQuantity(int id)
        {
            var item = await _dataContext.ProductColors.FindAsync(id);
            if (item == null) return NotFound();

            _dataContext.ProductColors.Remove(item);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Xóa số lượng màu thành công!";
            return RedirectToAction("ManageColorQuantity", new { id = item.ProductId });
        }

        [HttpGet]
        public async Task<IActionResult> ManageCapacityQuantity(int id)
        {
            var capacityQuantities = await _dataContext.ProductCapacities
                .Include(pc => pc.Capacity)
                .Where(pc => pc.ProductId == id)
                .ToListAsync();

            ViewBag.ProductId = id;
            return View(capacityQuantities);
        }

        [HttpGet]
        public async Task<IActionResult> AddCapacityQuantity(int id)
        {
            var product = await _dataContext.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewBag.ProductId = id;
            ViewBag.Capacities = await _dataContext.Capacities.ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCapacityQuantity(ProductCapacityModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Dữ liệu không hợp lệ!";
                ViewBag.ProductId = model.ProductId;
                ViewBag.Capacities = await _dataContext.Capacities.ToListAsync();
                return View(model);
            }

            // ✅ Kiểm tra Product và Capacity có tồn tại không
            var productExists = await _dataContext.Products.AnyAsync(p => p.Id == model.ProductId);
            var capacityExists = await _dataContext.Capacities.AnyAsync(c => c.Id == model.CapacityId);

            if (!productExists || !capacityExists)
            {
                TempData["error"] = "Sản phẩm hoặc dung lượng không tồn tại!";
                ViewBag.ProductId = model.ProductId;
                ViewBag.Capacities = await _dataContext.Capacities.ToListAsync();
                return View(model);
            }

            // ✅ Kiểm tra trùng dung lượng
            bool exists = await _dataContext.ProductCapacities
                .AnyAsync(pc => pc.ProductId == model.ProductId && pc.CapacityId == model.CapacityId);

            if (exists)
            {
                TempData["error"] = "Dung lượng đã tồn tại cho sản phẩm!";
                ViewBag.ProductId = model.ProductId;
                ViewBag.Capacities = await _dataContext.Capacities.ToListAsync();
                return View(model);
            }

            try
            {
                model.Id = 0;
                _dataContext.ProductCapacities.Add(model);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Thêm số lượng dung lượng thành công!";
                return RedirectToAction("ManageCapacityQuantity", new { id = model.ProductId });
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi khi lưu CSDL: " + (ex.InnerException?.Message ?? ex.Message);
                ViewBag.ProductId = model.ProductId;
                ViewBag.Capacities = await _dataContext.Capacities.ToListAsync();
                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> EditCapacityQuantity(int id)
        {
            var item = await _dataContext.ProductCapacities.FindAsync(id);
            if (item == null) return NotFound();

            ViewBag.Capacities = await _dataContext.Capacities.ToListAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCapacityQuantity(ProductCapacityModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Capacities = await _dataContext.Capacities.ToListAsync();
                return View(model);
            }

            var item = await _dataContext.ProductCapacities.FindAsync(model.Id);
            if (item == null)
            {
                TempData["error"] = "Không tìm thấy bản ghi cần sửa.";
                return RedirectToAction("ManageCapacityQuantity", new { id = model.ProductId });
            }

            var productExists = await _dataContext.Products.AnyAsync(p => p.Id == model.ProductId);
            var capacityExists = await _dataContext.Capacities.AnyAsync(c => c.Id == model.CapacityId);

            if (!productExists || !capacityExists)
            {
                TempData["error"] = "Sản phẩm hoặc dung lượng không tồn tại!";
                ViewBag.Capacities = await _dataContext.Capacities.ToListAsync();
                return View(model);
            }

            item.Quantity = model.Quantity;
            item.Status = model.Status;

            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Cập nhật số lượng dung lượng thành công!";
            return RedirectToAction("ManageCapacityQuantity", new { id = item.ProductId });
        }

        public async Task<IActionResult> DeleteCapacityQuantity(int id)
        {
            var item = await _dataContext.ProductCapacities.FindAsync(id);
            if (item == null) return NotFound();

            _dataContext.ProductCapacities.Remove(item);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Xóa số lượng dung lượng thành công!";
            return RedirectToAction("ManageCapacityQuantity", new { id = item.ProductId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVariant(ProductVariantModel model)
        {
            if (!ModelState.IsValid) return RedirectToAction("DetailStock", new { id = model.ProductId });

            _dataContext.ProductVariants.Add(model);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Thêm tồn kho (biến thể) thành công!";
            return RedirectToAction("DetailStock", new { id = model.ProductId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVariantQuantity(int id, int quantity)
        {
            var variant = await _dataContext.ProductVariants.FindAsync(id);
            if (variant == null) return NotFound();

            variant.Quantity = quantity;
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Cập nhật số lượng biến thể thành công!";
            return RedirectToAction("DetailStock", new { id = variant.ProductId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVariant(int id)
        {
            var variant = await _dataContext.ProductVariants.FindAsync(id);
            if (variant == null) return NotFound();

            _dataContext.ProductVariants.Remove(variant);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Xoá tồn kho biến thể thành công!";
            return RedirectToAction("DetailStock", new { id = variant.ProductId });
        }
    }
}
