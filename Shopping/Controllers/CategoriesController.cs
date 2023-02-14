using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Data;
using Shopping.Data.Entities;
using Shopping.Helpers;
using Vereyon.Web;
using static Shopping.Helpers.ModalHelper;

namespace Shopping.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly DataContext _context;
        private readonly IFlashMessage _flashMessage;

        public CategoriesController(DataContext context, IFlashMessage flashMessage)
        {
            _context = context;
            _flashMessage = flashMessage;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories
                .Include(c => c.ProductCategories)
                .ToListAsync());
        }

        #region NO SE USA MAS
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(category);
        //        try
        //        {
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));

        //        }
        //        catch (DbUpdateException dbUpdateException)
        //        {
        //            if (dbUpdateException.InnerException.Message.Contains("duplicate"))
        //            {
        //                //ModelState.AddModelError(string.Empty, "Ya existe una categorías con el mismo nombre.");
        //                _flashMessage.Danger("Ya existe una categorías con el mismo nombre.");
        //            }
        //            else
        //            {
        //                //ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
        //                _flashMessage.Danger(dbUpdateException.InnerException.Message);
        //            }
        //        }
        //        catch (Exception exception)
        //        {
        //            //ModelState.AddModelError(string.Empty, exception.Message);
        //            _flashMessage.Danger(exception.Message);
        //        }
        //    }
        //    return View(category);
        //}

        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.Categories == null)
        //    {
        //        return NotFound();
        //    }

        //    var category = await _context.Categories.FindAsync(id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(category);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, Category category)
        //{
        //    if (id != category.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(category);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch (DbUpdateException dbUpdateException)
        //        {
        //            if (dbUpdateException.InnerException.Message.Contains("duplicate"))
        //            {
        //                //ModelState.AddModelError(string.Empty, "Ya existe una cateoría con el mismo nombre.");
        //                _flashMessage.Danger("Ya existe una cateoría con el mismo nombre.");
        //            }
        //            else
        //            {
        //                //ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
        //                _flashMessage.Danger(dbUpdateException.InnerException.Message);
        //            }
        //        }
        //        catch (Exception exception)
        //        {
        //            //ModelState.AddModelError(string.Empty, exception.Message);
        //            _flashMessage.Danger(exception.Message);
        //        }
        //    }
        //    return View(category);
        //}
        #endregion
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        #region DELETE OLD
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.Categories == null)
        //    {
        //        return NotFound();
        //    }

        //    var category = await _context.Categories
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(category);
        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    Category category = await _context.Categories.FindAsync(id);
        //    _context.Categories.Remove(category);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));

        //    //if (_context.Categories == null)
        //    //{
        //    //    return Problem("Entity set 'DataContext.Countries'  is null.");
        //    //}
        //    //var category = await _context.Countries.FindAsync(id);
        //    //if (category != null)
        //    //{
        //    //    _context.Countries.Remove(category);
        //    //}

        //    //await _context.SaveChangesAsync();
        //    //return RedirectToAction(nameof(Index));
        //}
        #endregion

        [NoDirectAccess]
        public async Task<IActionResult> Delete(int? id)
        {
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                _flashMessage.Info("Registro borrado.");
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar la categoría porque tiene registros relacionados.");
            }

            return RedirectToAction(nameof(Index));
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                //return View(new Category());
                return View(new Category() { ProductCategories = new List<ProductCategory>() }); // le asignamos una nueva lista de productcategory por que el modelo no era invalido
            }
            else
            {
                Category category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound();
                }

                return View(category);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0) //Insert
                    {
                        _context.Add(category);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info("Registro creado.");
                    }
                    else //Update
                    {
                        _context.Update(category);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info("Registro actualizado.");
                    }
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("Ya existe una categoría con el mismo nombre.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                    return View(category);
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                    return View(category);
                }

                return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAll", _context.Categories.Include(c => c.ProductCategories).ToList()) });

            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddOrEdit", category) });
        }

    }
}
