using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payment.BusinessLayer.Abstract;

namespace Payment.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public IActionResult OrderList()
        {
            var values = _orderService.TGetList();
            return Ok(values);
        }
        [HttpPost("AddToCart")]
        public IActionResult AddToCart(int productId, int quantity)
        {
            try
            {
                _orderService.AddToCart(productId, quantity);
                return Ok("Ürün sepete eklendi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("RemoveFromCart")]
        public IActionResult RemoveFromCart(int productId)
        {
            try
            {
                _orderService.RemoveFromCart(productId);
                return Ok("Ürün sepetten silindi.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetCartItems")]
        public IActionResult GetCartItems()
        {
            try
            {
                var cartItems = _orderService.GetCartItems();
                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetCartTotalPrice")]
        public IActionResult GetCartTotalPrice()
        {
            try
            {
                var totalPrice = _orderService.GetCartTotalPrice();
                return Ok(totalPrice);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



    }
}
