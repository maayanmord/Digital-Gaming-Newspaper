using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DGN.Data;
using DGN.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace DGN.Controllers
{
    public class CommentsController : Controller
    {
        private readonly DGNContext _context;

        public CommentsController(DGNContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var dGNContext = _context.Comment.Include(c => c.RelatedArticle).Include(c => c.User);
            return View(await dGNContext.ToListAsync());
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Body,RelatedArticleId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.CreationTimestamp = DateTime.Now;
                comment.UserId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await _context.User.FirstOrDefaultAsync(u => u.Id == comment.UserId);
                _context.Add(comment);
                await _context.SaveChangesAsync();
                var CreationTimestamp = comment.CreationTimestamp.ToString("g"); // Send date with correct format
                return Json(new { comment.Id, comment.UserId, comment.Body, CreationTimestamp, user.FullName, user.ImageLocation, user.Username});
            }
            ViewData["RelatedArticleId"] = new SelectList(_context.Article, "Id", "Title", comment.RelatedArticleId);
            return BadRequest();
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string Body)
        {
            var comment = await _context.Comment.FirstOrDefaultAsync(e => e.Id == id);
            if (id != comment.Id)
            {
                return NotFound();
            }
            
            var userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = HttpContext.User.FindFirstValue(ClaimTypes.Role);
            if ((userId != comment.UserId) && !(userRole.Equals(UserRole.Admin.ToString())))
            {
                return Unauthorized();
            }
            if (Body.Length >= 5)
            {
                try
                {
                    comment.Body = Body;
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Json(comment);
            }
            return BadRequest();
        }

        // POST: Comments/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comment.FindAsync(id);
            var userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = HttpContext.User.FindFirstValue(ClaimTypes.Role);
            if ((userId != comment.UserId) && !(userRole.Equals(UserRole.Admin.ToString())))
            {
                return Unauthorized();
            }
            _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();
            return Json(comment);
        }

        private bool CommentExists(int id)
        {
            return _context.Comment.Any(e => e.Id == id);
        }

        [Authorize]
        public async Task<IActionResult> Search(string body, string fullname, string username)
        {
            return Json(await _context.Comment.Include(c => c.User).Where(c => (c.Body.Contains(body) || body == null) &&
                                                                         ((c.User.Firstname + " " + c.User.Lastname).Contains(fullname) || fullname == null) &&
                                                                         (c.User.Username.Contains(username) || username == null)).Select(c => new
                                                                         {
                                                                             c.Id,
                                                                             c.Body,
                                                                             c.User.FullName,
                                                                             c.User.Username,
                                                                             CreationTimestamp = c.CreationTimestamp.ToString("g"),
                                                                             c.RelatedArticle.Title,
                                                                             c.RelatedArticleId
                                                                         }).ToListAsync());
        }
    }
}