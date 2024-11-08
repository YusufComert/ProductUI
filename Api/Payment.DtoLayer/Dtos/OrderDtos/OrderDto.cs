using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.DtoLayer.Dtos.OrderDtos
{
    public class OrderDto
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public DateTime DateTime { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
