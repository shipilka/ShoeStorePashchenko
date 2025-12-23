using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeStoreLibrary.Contexts;
using ShoeStoreLibrary.DTOs;
using ShoeStoreLibrary.Models;
using ShoeStoreLibrary.Services;

namespace ShoeStoreWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        // Получение всех товаров
        [HttpGet]
        public IActionResult GetAllProducts()
        {
            var products = _productService.GetAllProducts();
            return Ok(products);
        }

        // Получение товара по артикулу
        [HttpGet("{articleNumber}")]
        public IActionResult GetProductByArticle(string articleNumber)
        {
            var product = _productService.GetProductByArticle(articleNumber);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // Добавление товара
        [HttpPost]
        [Authorize(Roles = "Администратор,Менеджер")]
        public IActionResult AddProduct([FromBody] ProductDto productDto)
        {
            _productService.AddProduct(productDto);
            return CreatedAtAction(nameof(GetProductByArticle), new { articleNumber = productDto.ProductId }, productDto);
        }

        // Обновление товара
        [HttpPut("{articleNumber}")]
        [Authorize(Roles = "Администратор,Менеджер")]
        public IActionResult UpdateProduct(string articleNumber, [FromBody] ProductDto productDto)
        {
            _productService.UpdateProduct(articleNumber, productDto);
            return NoContent();
        }

        // Удаление товара
        [HttpDelete("{articleNumber}")]
        [Authorize(Roles = "Администратор,Менеджер")]
        public IActionResult DeleteProduct(string articleNumber)
        {
            _productService.DeleteProduct(articleNumber);
            return NoContent();
        }
    }
}