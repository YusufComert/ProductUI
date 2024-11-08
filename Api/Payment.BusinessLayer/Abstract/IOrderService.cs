using Payment.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.BusinessLayer.Abstract
{
    public interface IOrderService : IGenericService<Order>
    {
        public void AddToCart(int productId, int quantity);
        public void RemoveFromCart(int productId);
        public List<Order> GetCartItems();
        public decimal GetCartTotalPrice();
    }
}
