using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GenericApp.Web.Data;
using GenericApp.Web.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericApp.Common.Requests;
using System.IO;
using System;
using GenericApp.Common.Helpers;
using GenericApp.Web.Helpers;
using GenericApp.Web.Models;
using GenericApp.Common.Responses;

namespace GenericApp.Web.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IFilesHelper _filesHelper;

        public ProductsController(DataContext context, IConverterHelper converterHelper, IImageHelper imageHelper,IFilesHelper filesHelper)
        {
            _context = context;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
            _filesHelper = filesHelper;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            List<ProductEntity> products = await _context.Products
                .Include(c => c.Category)
                .Include(p => p.ProductImages)
                .Include(s => s.State)
                .ToListAsync();

            return Ok(_converterHelper.ToProductResponse(products));
        }

        // POST: api/Products

        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] ProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var imageUrl = string.Empty;
            if (request.PhotoArray != null && request.PhotoArray.Length > 0)
            {
                var stream = new MemoryStream(request.PhotoArray);
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.jpg";
                var folder = "wwwroot\\images\\Products";
                var fullPath = $"~/images/Products/{file}";
                var response = _filesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    imageUrl = fullPath;
                }
            }

            var Product = new ProductEntity
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                Category= _context.Categories.FirstOrDefault(c=>c.Id==request.Category.Id),
                IsActive=true,
                Price=request.Price,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                State = _context.States.FirstOrDefault(c => c.Id == request.State.Id),
                ProductImages = new List<ProductImageEntity>(),
            };

            Product.ProductImages.Add(new ProductImageEntity
            {
                ImagePath= imageUrl
            });

            _context.Products.Add(Product);
            await _context.SaveChangesAsync();

            ProductResponse newProduct= _converterHelper.ToProductResponse(Product);

            return Ok(newProduct);
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct([FromRoute] int id, [FromBody] ProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != request.Id)
            {
                return BadRequest();
            }

            var oldProduct = await _context.Products.FindAsync(request.Id);
            if (oldProduct == null)
            {
                return BadRequest("La Luminaria no existe.");
            }

            var imageUrl = oldProduct.ImageFullPath;
            if (request.PhotoArray != null && request.PhotoArray.Length > 0)
            {
                var stream = new MemoryStream(request.PhotoArray);
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.jpg";
                var folder = "wwwroot\\images\\Products";
                var fullPath = $"~/images/Products/{file}";
                var response = _filesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    imageUrl = fullPath;
                }
            }

            oldProduct.Id = request.Id;
            oldProduct.Name = request.Name;
            oldProduct.Description = request.Description;
            oldProduct.Latitude = request.Latitude;
            oldProduct.Longitude = request.Longitude;
            oldProduct.Category = _context.Categories.FirstOrDefault(c => c.Id == request.Category.Id);
            oldProduct.IsActive = true;
            oldProduct.Price = request.Price;
            oldProduct.State = _context.States.FirstOrDefault(c => c.Id == request.State.Id);

            _context.Products.Update(oldProduct);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            var Product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);
            if (Product == null)
            {
                return this.NotFound();
            }

            _context.Products.Remove(Product);
            await _context.SaveChangesAsync();
            return Ok("Producto borrado");
        }
    }
}