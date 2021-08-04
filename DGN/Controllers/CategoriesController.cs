using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DGN.Data;
using DGN.Models;
using Microsoft.AspNetCore.Authorization;

namespace DGN.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly DGNContext _context;

        public CategoriesController(DGNContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Category.ToListAsync());
        }

        // GET: All Categories
        public async Task<IActionResult> GetAll()
        {
            return Json(await _context.Category.ToListAsync());
        }


        // GET: Categories/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .Include(c => c.Articles)
                .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,CategoryName")] Category category)
        {
            if (CategoryExists(category.CategoryName))
            {
                ViewData["Error"] = "Category is already exist!";
                return View(category);
            }
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryName")] Category category)
        {
            var oldCategory = await _context.Category.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if ((oldCategory != null) && (id != category.Id))
            {
                return NotFound();
            }

            if (oldCategory.CategoryName != category.CategoryName)
            {
                if (CategoryExists(category.CategoryName))
                {
                    ViewData["Error"] = "Category already exist!";
                    return View(category);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Category.FindAsync(id);
            _context.Category.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Category.Any(e => e.Id == id);
        }
        private bool CategoryExists(string categoryName)
        {
            return _context.Category.Any(e => e.CategoryName == categoryName);
        }

        public async Task<IActionResult> Search(string categoryName)
        {
            return Json(await _context.Category.Where(c => (c.CategoryName.Contains(categoryName) || categoryName == null)).ToListAsync());
        }
    }
}
