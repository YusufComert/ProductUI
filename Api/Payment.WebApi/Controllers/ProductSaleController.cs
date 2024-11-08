using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payment.BusinessLayer.Abstract;

namespace Payment.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductSaleController : ControllerBase
    {
        private readonly IProductSaleService _productSaleService;

        public ProductSaleController(IProductSaleService productSaleService)
        {
            _productSaleService = productSaleService;
        }

        [HttpGet]
        public IActionResult ProductSaleList()
        {
            var values = _productSaleService.TGetList();
            return Ok(values);
        }

        [HttpGet("{id}")]
        public IActionResult GetProductSale(int id)
        {
            var value = _productSaleService.TGetByID(id);
            return Ok(value);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteProductSale(int id)
        {
            var value = _productSaleService.TGetByID(id);
            _productSaleService.TDelete(value);
            return Ok();
        }
    }
}
