using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DGN.Data;
using DGN.Models;

namespace DGN.Controllers
{
    public class PasswordsController : Controller
    {
        private readonly DGNContext _context;

        public PasswordsController(DGNContext context)
        {
            _context = context;
        }

        // GET: Passwords
        public async Task<IActionResult> Index()
        {
            var dGNContext = _context.Password.Include(p => p.User);
            return View(await dGNContext.ToListAsync());
        }

        // GET: Passwords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var password = await _context.Password
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (password == null)
            {
                return NotFound();
            }

            return View(password);
        }

        // GET: Passwords/Create
        public IActionResult Create()
        {
            ViewData["Id"] = new SelectList(_context.User, "Id", "Email");
            return View();
        }

        // POST: Passwords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Hash,Salt")] Password password)
        {
            if (ModelState.IsValid)
            {
                _context.Add(password);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id"] = new SelectList(_context.User, "Id", "Email", password.Id);
            return View(password);
        }

        // GET: Passwords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var password = await _context.Password.FindAsync(id);
            if (password == null)
            {
                return NotFound();
            }
            ViewData["Id"] = new SelectList(_context.User, "Id", "Email", password.Id);
            return View(password);
        }

        // POST: Passwords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Hash,Salt")] Password password)
        {
            if (id != password.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(password);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PasswordExists(password.Id))
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
            ViewData["Id"] = new SelectList(_context.User, "Id", "Email", password.Id);
            return View(password);
        }

        // GET: Passwords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var password = await _context.Password
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (password == null)
            {
                return NotFound();
            }

            return View(password);
        }

        // POST: Passwords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var password = await _context.Password.FindAsync(id);
            _context.Password.Remove(password);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PasswordExists(int id)
        {
            return _context.Password.Any(e => e.Id == id);
        }
    }
}
