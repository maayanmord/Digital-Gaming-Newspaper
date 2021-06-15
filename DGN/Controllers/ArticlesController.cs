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
    public class ArticlesController : Controller
    {
        private readonly DGNContext _context;

        public ArticlesController(DGNContext context)
        {
            _context = context;
        }

        // GET: Articles
        public async Task<IActionResult> Index()
        {
            var dGNContext = _context.Article.Include(a => a.Category).Include(a => a.User);
            return View(await dGNContext.ToListAsync());
        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .Include(a => a.Category)
                .Include(a => a.User)
                .Include(a => a.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Articles/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName");
            /*ViewData["UserId"] = new SelectList(_context.User, "Id", "Email");*/
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Body,ImageLocation,CategoryId")] Article article)
        {
            if (ModelState.IsValid && !ArticleExists(article.Title))
            {
                article.CreationTimestamp = DateTime.Now;
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName", article.CategoryId);
            /*ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", article.UserId);*/
            return View(article);
        }

        // GET: Articles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName", article.CategoryId);
            /*ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", article.UserId);*/
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Body,ImageLocation,CategoryId")] Article newArticle)
        {
            var currArticle = await _context.Article.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if ((currArticle == null) || (id != newArticle.Id))
            {
                return NotFound();
            }

            bool NotDuplicatedTitle = true;
            if (currArticle.Title != newArticle.Title)
            {
                NotDuplicatedTitle = !ArticleExists(newArticle.Title);
            }

            if ((ModelState.IsValid) && (NotDuplicatedTitle))
            {
                try
                {
                    _context.Update(newArticle);
                    newArticle.CreationTimestamp = currArticle.CreationTimestamp;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(newArticle.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName", newArticle.CategoryId);
            /*ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", article.UserId);*/
            return View(newArticle);
        }

        // GET: Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .Include(a => a.Category)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Article.FindAsync(id);
            _context.Article.Remove(article);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Article.Any(e => e.Id == id);
        }

        private bool ArticleExists(string title)
        {
            return _context.Article.Any(e => e.Title == title);
        }

        public async Task<IActionResult> Search(string queryTitle)
        {
            return Json(await _context.Article.Where(a => (a.Title.Contains(queryTitle))).ToListAsync());
        }
    }
}
