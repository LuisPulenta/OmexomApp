using GenericApp.Web.Data;
using GenericApp.Web.Data.Entities;
using GenericApp.Web.Helpers;
using GenericApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
//using Vereyon.Web;

namespace GenericApp.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly DataContext _context;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;
  //      private readonly IFlashMessage _flashMessage;

        public CategoriesController(DataContext context,
            IImageHelper imageHelper,
            IConverterHelper converterHelper)
    //        IFlashMessage flashMessage
    
        {
            _context = context;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
      //      this._flashMessage = flashMessage;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;

                if (model.ImageFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile, "Categories");
                }

                var category = _converterHelper.ToCategoryEntity(model, path, true);
                _context.Add(category);
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Esta Categoría ya existe");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    }
                }
            }
            return View(model);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CategoryEntity Category = await _context.Categories.FindAsync(id);
            if (Category == null)
            {
                return NotFound();
            }

            CategoryViewModel model = _converterHelper.ToCategoryViewModel(Category);
            return View(model);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (ModelState.IsValid)
                {
                    var path = model.ImagePath;

                    if (model.ImageFile != null)
                    {
                        path = await _imageHelper.UploadImageAsync(model.ImageFile, "Categories");
                    }

                    CategoryEntity category = _converterHelper.ToCategoryEntity(model, path, false);
                    _context.Update(category);
                    try
                    {
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException.Message.Contains("duplicate"))
                        {
                            ModelState.AddModelError(string.Empty, "Esta Categoría ya existe");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                        }
                    }
                }
            }
            return View(model);
        }

        // POST: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CategoryEntity category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
 //               _flashMessage.Confirmation("La categoría fue borrada.");
            }
            catch
            {
   //             _flashMessage.Danger("No se puede borrar la categoría porque tiene registros relacionados.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}