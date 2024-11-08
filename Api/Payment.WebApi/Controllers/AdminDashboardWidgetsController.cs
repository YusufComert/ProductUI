using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Payment.BusinessLayer.Abstract;

namespace Payment.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminDashboardWidgetsController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IProductService _productService;

        public AdminDashboardWidgetsController(UserManager<AppUser> userManager, IProductService productService)
        {
            _userManager = userManager;
            _productService = productService;
        }

        [HttpGet("UserCount")]
        public IActionResult GetUserCount()
        {
            var users = _userManager.Users.Count();
            return Ok(users);
        }

        [HttpGet("ProductCount")]
        public IActionResult GetProductCount()
        {
            var productCount = _productService.GetTotalProducts();
            return Ok(productCount);
        }

        [HttpGet("TotalProductValue")]
        public IActionResult GetTotalProductValue()
        {
            var totalProductValue = _productService.TGetTotalProductValue();
            return Ok(totalProductValue);
        }

    }
}
