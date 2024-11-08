namespace Payment.WebUI.DTOs.OrderDtos
{
    public class ResultOrderDto
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public DateTime DateTime { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
