using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.EntityLayer.Concrete
{
    public class ProductSale
    {
        public int ID { get; set; }
        public Product? Product { get; set; }
        public int ProductId { get; set; }
        public int AdressId { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? DeliveryStatus { get; set; }
        public string DeliveryType { get; set; }
        public string PaymentStatus { get; set; }
        public int TotalPrice { get; set; }
        public int ProductCount { get; set; }
        public int? SellRating { get; set; }
        public DateTime? ReturnRequestDate { get; set; }
        public string? ReturnReason { get; set; }
    }
}
