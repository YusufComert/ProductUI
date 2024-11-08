using Microsoft.AspNetCore.Mvc;
using Payment.BusinessLayer.Abstract;
using Payment.WebUI.DTOs.CategoryDtos;

namespace Payment.WebUI.ViewComponents.Layout
{
    public class _LayoutHeaderPartial : ViewComponent
    {
        private readonly ICategoryService _categoryService;

        public _LayoutHeaderPartial(ICategoryService categoryService) 
        {
            _categoryService = categoryService;
        }
        public IViewComponentResult Invoke()
        {
            var categories = _categoryService.TGetList(); // Bu List<Category> döndürüyor.

            // categories listesini ResultCategoryDto listesine dönüştür
            var model = categories.Select(category => new ResultCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                // Diğer gerekli alanlar
            }).ToList();

            return View(model);

        }
    }
}
