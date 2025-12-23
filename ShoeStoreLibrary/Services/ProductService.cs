using ShoeStoreLibrary.Contexts;
using ShoeStoreLibrary.DTOs;
using ShoeStoreLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoeStoreLibrary.Services
{
    public class ProductService
    {
        private readonly ShoeStoreContext _context;

        public ProductService(ShoeStoreContext context)
        {
            _context = context;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }

        public Product GetProductByArticle(string articleNumber)
        {
            return _context.Products.FirstOrDefault(p => p.ProductId == articleNumber);
        }

        public void AddProduct(ProductDto productDto)
        {
            // Проверка существование поставщика
            var supplierExists = _context.Suppliers.Any(s => s.SupplierId == productDto.SupplierId);
            if (!supplierExists)
            {
                throw new Exception("Указанный поставщик не существует.");
            }

            var product = new Product
            {
                ProductId = productDto.ProductId,
                ProductNameId = productDto.ProductNameId,
                Price = productDto.Price,
                SupplierId = productDto.SupplierId,
                ManufacturerId = productDto.ManufacturerId,
                CategoryId = productDto.CategoryId,
                ActiveDiscount = productDto.ActiveDiscount,
                StockQuantity = productDto.StockQuantity,
                UnitOfMeasureId = productDto.UnitOfMeasureId,
                ProductDescription = productDto.ProductDescription
            };

            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void UpdateProduct(string articleNumber, ProductDto productDto)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == articleNumber);
            if (product != null)
            {
                // Обновляем все поля товара
                product.ProductNameId = productDto.ProductNameId;
                product.Price = productDto.Price;
                product.SupplierId = productDto.SupplierId;                  
                product.ManufacturerId = productDto.ManufacturerId;        
                product.CategoryId = productDto.CategoryId;                
                product.ActiveDiscount = productDto.ActiveDiscount;        
                product.StockQuantity = productDto.StockQuantity;          
                product.UnitOfMeasureId = productDto.UnitOfMeasureId;    
                product.ProductDescription = productDto.ProductDescription; 

                // Проверяем, какие свойства подлежат обновлению
                _context.SaveChanges(); // Сохраняем изменения
            }
        }

        public void DeleteProduct(string articleNumber)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == articleNumber);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }
    }
}
