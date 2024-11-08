using Microsoft.AspNetCore.Http;
using Payment.BusinessLayer.Abstract;
using Payment.DataAccessLayer.Abstract;
using Payment.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.BusinessLayer.Concrete
{
    public class OrderManager : IOrderService
    {
        private readonly IOrderDal _orderDal;
        private readonly IProductService _productService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderManager(IOrderDal orderDal, IProductService productService, IHttpContextAccessor httpContextAccessor)
        {
            _orderDal = orderDal;
            _productService = productService;
            _httpContextAccessor = httpContextAccessor;
        }

        public void TDelete(Order t)
        {
            throw new NotImplementedException();
        }

        public Order TGetByID(int id)
        {
            return _orderDal.GetByID(id);
        }

        public List<Order> TGetList()
        {
            return _orderDal.GetList();
        }

        public void TInsert(Order t)
        {
            throw new NotImplementedException();
        }

        public void TUpdate(Order t)
        {
            throw new NotImplementedException();
        }
        public void AddToCart(int productId, int quantity)
        {
            // Ürünü ProductService'den al
            var product = _productService.TGetByID(productId);

            if (product != null)
            {
                // Oturum açmış kullanıcının ID'sini al
                //var userId = _httpContextAccessor.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;

                var order = new Order
                {
                    Product = product,
                    ProductID = productId,
                    ProductName = product.Title,
                    DateTime = DateTime.Now,
                    TotalPrice = product.Price * quantity,
                    Quantity = quantity,
                    /*UserID = int.Parse(userId)*/ // Oturum açmış kullanıcının ID'sini al ve ata
                };

                _orderDal.Insert(order); // Siparişi ekle
            }
            else
            {
                throw new Exception("Ürün bulunamadı.");
            }
        }
        public void RemoveFromCart(int productId)
        {
            // Oturum açmış kullanıcının ID'sini al
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                throw new Exception("Kullanıcı oturumu bulunamadı.");
            }

            // Mevcut siparişi bul
            var existingOrder = _orderDal.GetList().FirstOrDefault(o => o.ProductID == productId && o.UserID == int.Parse(userId));

            if (existingOrder != null)
            {
                _orderDal.Delete(existingOrder); // Siparişi sil
            }
            else
            {
                throw new Exception("Ürün sepette bulunamadı.");
            }
        }
        public List<Order> GetCartItems()
        {
            // Oturum açmış kullanıcının ID'sini al
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                throw new Exception("Kullanıcı oturumu bulunamadı.");
            }

            return _orderDal.GetList().Where(o => o.UserID == int.Parse(userId)).ToList();
        }
        public decimal GetCartTotalPrice()
        {
            // Oturum açmış kullanıcının ID'sini al
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                throw new Exception("Kullanıcı oturumu bulunamadı.");
            }

            return _orderDal.GetList()
                            .Where(o => o.UserID == int.Parse(userId))
                            .Sum(o => o.TotalPrice);
        }


    }
}
