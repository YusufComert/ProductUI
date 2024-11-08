using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.EntityLayer.Concrete
{
    public class Order
    {
        public int OrderID { get; set; }
        public AppUser? User { get; set; }
        public int? UserID { get; set; }
        public Product? Product { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public DateTime DateTime { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }
    }
}
