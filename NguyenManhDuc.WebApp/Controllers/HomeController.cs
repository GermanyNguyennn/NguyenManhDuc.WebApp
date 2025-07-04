using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenManhDuc.WebApp.Models;
using NguyenManhDuc.WebApp.Models.ViewModels;
using NguyenManhDuc.WebApp.Repository;

namespace NguyenManhDuc.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }
        public async Task<IActionResult> Index(string sort_by = "", string startprice = "", string endprice = "")
        {
            var query = _dataContext.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Company)
            .AsQueryable();

            if (!string.IsNullOrEmpty(startprice) && !string.IsNullOrEmpty(endprice))
            {
                if (decimal.TryParse(startprice, out decimal startPriceVal) &&
                    decimal.TryParse(endprice, out decimal endPriceVal))
                {
                    query = query.Where(p => p.Price >= startPriceVal && p.Price <= endPriceVal);
                }
            }

            query = sort_by switch
            {
                "price_increase" => query.OrderBy(p => p.Price),
                "price_decrease" => query.OrderByDescending(p => p.Price),
                "price_newest" => query.OrderByDescending(p => p.Id),
                "price_oldest" => query.OrderBy(p => p.Id),
                _ => query.OrderByDescending(p => p.Id),
            };
            var products = await query.ToListAsync();

            ViewBag.Sliders = await _dataContext.Sliders
                .Where(s => s.Status == 1)
                .ToListAsync(); ;
            ViewBag.sort_key = sort_by;
            ViewBag.count = products.Count;

            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statuscode)
        {
            if (statuscode == 404)
            {
                return View("NotFound");
            }
            else
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public async Task<IActionResult> Contact()
        {
            var contact = await _dataContext.Contacts.FirstOrDefaultAsync();
            return View(contact);
        }
    }
}
