using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenManhDuc.WebApp.Repository.Validation;

namespace NguyenManhDuc.WebApp.Repository.Components
{
    public class CategoriesViewComponent : ViewComponent
    {
        private readonly DataContext _dataContext;
        public CategoriesViewComponent(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IViewComponentResult> InvokeAsync() => View(await _dataContext.Categories.ToListAsync());
    }
}
