using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GenericApp.Web.Data;
using GenericApp.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericApp.Web.Data.Entities;
using GenericApp.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace GenericApp.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly DataContext _context;
        private readonly IImageHelper _imageHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IConverterHelper _converterHelper;

        public ProductsController(DataContext context, IImageHelper imageHelper, ICombosHelper combosHelper, IConverterHelper converterHelper)
        {
            _context = context;
            _imageHelper = imageHelper;
            _combosHelper = combosHelper;
            _converterHelper = converterHelper;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Products
                .Include(c => c.Category)
                .Include(p => p.ProductImages)
                .Include(s=>s.State)
                .ToListAsync());
        }

        public IActionResult Create()
        {
            ProductViewModel model = new ProductViewModel
            {
                Categories = _combosHelper.GetComboCategories(),
                States = _combosHelper.GetComboStates(),
                IsActive = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;
                try
                {
                    ProductEntity product = await _converterHelper.ToProductAsync(model, true);

                    if (model.ImageFile != null)
                    {
                        path = await _imageHelper.UploadImageAsync(model.ImageFile, "Products");

                        product.ProductImages = new List<ProductImageEntity>
                        {
                            new ProductImageEntity { ImagePath = path }
                        };
                    }

                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Hay un producto con el mismo nombre.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }
            model.Categories = _combosHelper.GetComboCategories();
            model.States = _combosHelper.GetComboStates();
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductEntity product = await _context.Products
                .Include(c => c.Category)
                .Include(p => p.ProductImages)
                .Include(s => s.State)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            ProductViewModel model = _converterHelper.ToProductViewModel(product);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            var path = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    ProductEntity product = await _converterHelper.ToProductAsync(model, false);

                    if (model.ImageFile != null)
                    {
                        path = await _imageHelper.UploadImageAsync(model.ImageFile, "Products");

                        if (product.ProductImages == null)
                        {
                            product.ProductImages = new List<ProductImageEntity>();
                        }

                        product.ProductImages.Add(new ProductImageEntity { ImagePath = path });
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Este producto ya existe.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }
            model.Categories = _combosHelper.GetComboCategories();
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductEntity product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductEntity product = await _context.Products
                .Include(c => c.Category)
                .Include(p => p.ProductImages)
                .Include(s => s.State)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public async Task<IActionResult> AddImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductEntity product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            AddProductImageViewModel model = new AddProductImageViewModel { ProductId = product.Id };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImage(AddProductImageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;
                ProductEntity product = await _context.Products
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.Id == model.ProductId);
                if (product == null)
                {
                    return NotFound();
                }

                try
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile, "Products");
                    if (product.ProductImages == null)
                    {
                        product.ProductImages = new List<ProductImageEntity>();
                    }

                    product.ProductImages.Add(new ProductImageEntity { ImagePath = path });
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction($"{nameof(Details)}/{product.Id}");

                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteImage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductImageEntity productImage = await _context.ProductImages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productImage == null)
            {
                return NotFound();
            }

            ProductEntity product = await _context.Products.FirstOrDefaultAsync(p => p.ProductImages.FirstOrDefault(pi => pi.Id == productImage.Id) != null);
            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();
            return RedirectToAction($"{nameof(Details)}/{product.Id}");
        }
    }
}