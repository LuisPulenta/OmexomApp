using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GenericApp.Web.Data;
using GenericApp.Common.Responses;
using GenericApp.Web.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using GenericApp.Common.Helpers;
using GenericApp.Common.Requests;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace GenericApp.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductImagesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IFilesHelper _filesHelper;

        public ProductImagesController(DataContext context, IFilesHelper filesHelper)
        {
            _context = context;
            _filesHelper = filesHelper;
        }

        // POST: api/Products

        [HttpPost]
        public async Task<IActionResult> PostProductImage([FromBody] ProductImageRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Foto
            var imageUrl1 = string.Empty;
            var stream = new MemoryStream(request.ImageArray);
            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}.jpg";
            var folder = "wwwroot\\images\\Products";
            var fullPath = $"~/images/Products/{file}";
            var response = _filesHelper.UploadPhoto(stream, folder, file);

            if (response)
            {
                imageUrl1 = fullPath;
            }

            ProductEntity Product2 = await _context.Products
                .FirstOrDefaultAsync(o => o.Id == request.Product.Id);

            var ProductImage = new ProductImageEntity
            {
                Id = request.Id,
                ImagePath = imageUrl1,
                Product = Product2
            };

            _context.ProductImages.Add(ProductImage);
            await _context.SaveChangesAsync();

            ProductImageResponse2 ProductImageResponse2 = new ProductImageResponse2
            {
                Id = ProductImage.Id,
                ImageUrl = ProductImage.ImagePath,
            };

            return Ok(ProductImageResponse2);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductImage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            var ProductImage = await _context.ProductImages
                .FirstOrDefaultAsync(p => p.Id == id);
            if (ProductImage == null)
            {
                return this.NotFound();
            }

            _context.ProductImages.Remove(ProductImage);
            await _context.SaveChangesAsync();
            return Ok("Imagen de Producto borrada");
        }
    }
}