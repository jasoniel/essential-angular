﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServerApp.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductValuesController : Controller
    {
        private DataContext context;

        public ProductValuesController(DataContext ctx)
        {
                context = ctx;
        }

        [HttpGet("{id}")]
        public Product GetProduct(long id)
        {
            Thread.Sleep(1000);
            Product result = context.Products
                .Include(p => p.Supplier).ThenInclude(p => p.Products)
                .Include(p => p.Ratings)
                .FirstOrDefault(p => p.ProductId == id);

            if(result != null)
            {
                if(result.Supplier != null)
                {
                    result.Supplier.Products = result.Supplier.Products.Select(p =>
                       new Product
                       {
                           ProductId = p.ProductId,
                           Name = p.Name,
                           Category = p.Description,
                           Price = p.Price
                       });
                }
                if(result.Ratings != null)
                {
                    result.Ratings.ForEach(r => r.Product = null);
                }
            }

            return result;
        }   

        [HttpGet]
        public IEnumerable<Product> GetProducts(string category, string search, bool related = false)
        {
            IQueryable<Product> query = context.Products;

            if (!string.IsNullOrWhiteSpace(category))
            {
                string catLower = category.ToLower();
                query = query.Where(p => p.Category.ToLower().Contains(catLower));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                string searchLower = search.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(searchLower) || p.Description.ToLower().Contains(searchLower));
            }

            if (related)
            {
                query = query.Include(p => p.Supplier).Include(p => p.Ratings);
                List<Product> data = query.ToList();

                data.ForEach(p =>
                {
                    if (p.Supplier != null)
                    {
                        p.Supplier.Products = null;
                    }
                    if (p.Ratings != null)
                    {
                        p.Ratings.ForEach(r => r.Product = null);
                    }
                });

                return data;
            }
            else
            {
                return query;
            }
        }
    }
}
