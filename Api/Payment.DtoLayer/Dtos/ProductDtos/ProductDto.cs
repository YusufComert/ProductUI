﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.DtoLayer.Dtos.ProductDtos
{
    public class ProductDto
    {
        public int ProductID { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountRate { get; set; }
        public int Stock { get; set; }
        public string? CoverImage { get; set; }
        public string? FileCover { get; set; }
        public decimal? Rating { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string CreateUser { get; set; }
        public string? UpdateUser { get; set; }

    }
}
