using NguyenManhDuc.WebApp.Models;

namespace NguyenManhDuc.WebApp.Areas.Admin.Models.ViewModels
{
    public class AdminProductStockViewModel
    {
        public ProductModel Product { get; set; }

        public List<ColorStockViewModel> Colors { get; set; }
        public List<CapacityStockViewModel> Capacities { get; set; }
        public List<VariantStockViewModel> Variants { get; set; }
    }

    public class ColorStockViewModel
    {
        public string ColorName { get; set; }
        public int Quantity { get; set; }
    }

    public class CapacityStockViewModel
    {
        public string CapacityName { get; set; }
        public int Quantity { get; set; }
    }

    public class VariantStockViewModel
    {
        public string ColorName { get; set; }
        public string CapacityName { get; set; }
        public int Quantity { get; set; }
    }
}
