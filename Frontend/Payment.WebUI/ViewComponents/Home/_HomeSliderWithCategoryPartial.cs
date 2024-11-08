using Microsoft.AspNetCore.Mvc;
using Payment.BusinessLayer.Abstract;
using Payment.WebUI.DTOs.CategoryDtos;
using Payment.WebUI.DTOs.ProductDtos;
using Payment.WebUI.Models;

namespace Payment.WebUI.ViewComponents.Index
{
    public class _HomeSliderWithCategoryPartial : ViewComponent
    {
        private readonly IHttpClientFactory _clientFactory;

        private readonly ICategoryService _categoryService;

        private readonly IProductService  _productService;
        public _HomeSliderWithCategoryPartial(IHttpClientFactory clientFactory, ICategoryService categoryService, IProductService productService)
        {
            _clientFactory = clientFactory;
            _categoryService = categoryService;
            _productService = productService;
        }

        public IViewComponentResult Invoke()
        {
            var categories = _categoryService.TGetList(); // List<Category>
            var products = _productService.TGetList(); // List<Product>

            // Category listesini CategoryDto'ya dönüştür
            var categoryDtos = categories.Select(category => new ProductWithCategory.CategoryDto
            {
                CategoryId = category.Id,
                CategoryName = category.Name
            }).ToList();

            // Product listesini ProductViewModel'e dönüştür
            var productDtos = products.Select(product => new ProductViewModel
            {
                ProductID = product.ProductID,
                CategoryId = product.CategoryId,
                CategoryName = categoryDtos.FirstOrDefault(c => c.CategoryId == product.CategoryId)?.CategoryName,
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                DiscountRate = product.DiscountRate,
                Stock = product.Stock,
                CoverImage = product.CoverImage,
                Rating = product.Rating,
                CreateTime = product.CreateTime,
                UpdateTime = product.UpdateTime,
                CreateUser = product.CreateUser,
                UpdateUser = product.UpdateUser
            }).ToList();

            // ViewModel'i doldur
            var model = new ProductWithCategory.ProductCategoryViewModel()
            {
                Categories = categoryDtos,
                Products = productDtos
            };

            // View'a modeli gönder
            return View(model);
        }
    }
}
