namespace Payment.WebUI.Models
{
    public class ProductWithCategory
    {
        public class ProductCategoryViewModel
        {
            public List<CategoryDto> Categories { get; set; }
            public List<ProductViewModel> Products { get; set; }
        }

        public class CategoryDto
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
        }
    }
}
