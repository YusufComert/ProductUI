﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Payment.DataAccessLayer.Abstract;
using Payment.DataAccessLayer.Concrete;
using Payment.DataAccessLayer.Repositories;
using Payment.DtoLayer.Dtos.ProductDtos;
using Payment.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.DataAccessLayer.EntityFramework
{
    public class EfProductDal : GenericRepository<Product>, IProductDal
    {
        private readonly Context _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EfProductDal(Context context, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<ProductDto> GetLast3Product()
        {
            var context = new Context();
            var values = context.Products.Include(x => x.Category).OrderByDescending(x => x.ProductID).Take(3).Select(y => new ProductDto
            {
                ProductID = y.ProductID,
                CategoryName = y.Category.Name,
                CategoryId = y.Category.Id,
                Title = y.Title,
                Description = y.Description,
                Price = y.Price,
                DiscountRate = y.DiscountRate,
                Stock = y.Stock,
                CoverImage = y.CoverImage,
                Rating = y.Rating,
                IsActive = y.IsActive,
                CreateTime = y.CreateTime,
                UpdateTime = y.UpdateTime,
                CreateUser = y.CreateUser,
                UpdateUser = y.UpdateUser,
                //FileCover=y.FileCover
            }).ToList();
            return values;
        }

        public IEnumerable<ProductDto> GetProducts(int page, int pageSize)
        {
            return _context.Products
             .OrderBy(p => p.ProductID)
             .Skip((page - 1) * pageSize)
             .Take(pageSize)
             .Select(p => new ProductDto
             {
                 ProductID = p.ProductID,
                 CategoryName = p.Category.Name,
                 CategoryId = p.Category.Id,
                 Title = p.Title,
                 Description = p.Description,
                 Price = p.Price,
                 DiscountRate = p.DiscountRate,
                 Stock = p.Stock,
                 CoverImage = p.CoverImage,
                 Rating = p.Rating,
                 IsActive = p.IsActive,
                 CreateTime = p.CreateTime,
                 UpdateTime = p.UpdateTime,
                 CreateUser = p.CreateUser,
                 UpdateUser = p.UpdateUser,
                 //FileCover=p.FileCover
             })
             .ToList();
        }

        public List<ProductDto> GetProductWithCategoryName()
        {
            var context = new Context();
            List<ProductDto> value = new List<ProductDto>();
            var values = context.Products.Include(x => x.Category).Select(y => new ProductDto
            {
                ProductID = y.ProductID,
                CategoryName = y.Category.Name,
                CategoryId = y.Category.Id,
                Title = y.Title,
                Description = y.Description,
                Price = y.Price,
                DiscountRate = y.DiscountRate,
                Stock = y.Stock,
                CoverImage = y.CoverImage,
                Rating = y.Rating,
                IsActive = y.IsActive,
                CreateTime = y.CreateTime,
                UpdateTime = y.UpdateTime,
                CreateUser = y.CreateUser,
                UpdateUser = y.UpdateUser,
                //FileCover=y.FileCover

            });
            return value;
        }

        public ProductDto GetProductWithCategoryNameById(int id)
        {
            var context = new Context();
            var values = context.Products.Include(x => x.Category).Where(x => x.ProductID == id).Select(y => new ProductDto
            {
                ProductID = y.ProductID,
                CategoryName = y.Category.Name,
                CategoryId = y.Category.Id,
                Title = y.Title,
                Description = y.Description,
                Price = y.Price,
                DiscountRate = y.DiscountRate,
                Stock = y.Stock,
                CoverImage = y.CoverImage,
                Rating = y.Rating,
                IsActive = y.IsActive,
                CreateTime = y.CreateTime,
                UpdateTime = y.UpdateTime,
                CreateUser = y.CreateUser,
                UpdateUser = y.UpdateUser,
                //FileCover=y.FileCover
            }).FirstOrDefault();
            return values;
        }

        public List<ProductDto> GetTopThreeProductsByRating()
        {
            var context = new Context();
            var values = context.Products.Include(x => x.Category).Where(x => x.IsActive == true).OrderByDescending(x => x.Rating).Take(3).Select(y => new ProductDto
            {
                ProductID = y.ProductID,
                CategoryName = y.Category.Name,
                CategoryId = y.Category.Id,
                Title = y.Title,
                Description = y.Description,
                Price = y.Price,
                DiscountRate = y.DiscountRate,
                Stock = y.Stock,
                CoverImage = y.CoverImage,
                Rating = y.Rating,
                IsActive = y.IsActive,
                CreateTime = y.CreateTime,
                UpdateTime = y.UpdateTime,
                CreateUser = y.CreateUser,
                UpdateUser = y.UpdateUser,
                //FileCover=y.FileCover
            }).ToList();
            return values;
        }

        public int GetTotalProducts()
        {
            return _context.Products.Count();
        }

        public decimal GetTotalProductValue()
        {
            //var user = _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            //var userName = user.Result.Name;
            //var context = new Context();
            //return context.Products.Where(x => x.IsActive == false && x.CreateUser == userName).Sum(x => x.Price);
           
            var context = new Context();
            return context.Products.Where(x => x.IsActive == false).Sum(x => x.Price);
        }

        public string ProductIsActiveChange(int id)
        {
            var context = new Context();
            var values = context.Products.Find(id);
            values.IsActive = true;
            context.SaveChanges();
            if (values.IsActive == true)
            {
                return "Aktif";
            }
            return "Pasif";
        }
        public string ProductIsActiveChangeCancel(int id)
        {
            var context = new Context();
            var values = context.Products.Find(id);
            values.IsActive = false;
            context.SaveChanges();
            if (values.IsActive == false)
            {
                return "Pasif";
            }

            return "Aktif";
        }

    }
}
