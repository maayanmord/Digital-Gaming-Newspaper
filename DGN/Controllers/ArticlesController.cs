﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DGN.Data;
using DGN.Models;
using DGN.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace DGN.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly DGNContext _context;
        private readonly ImagesService _service;
        private string DEFAULT_IMAGE;

        public ArticlesController(DGNContext context, ImagesService service)
        {
            _context = context;
            _service = service;
            DEFAULT_IMAGE = _service.CLIENT_IMAGES_LOCATION + "DefaultArticleImage.jpg";
        }

        // GET: Articles
        public async Task<IActionResult> Index()
        {
            var dGNContext = _context.Article.Include(a => a.Category).Include(a => a.User).OrderByDescending(a => a.CreationTimestamp);
            return View(await dGNContext.Take(5).ToListAsync());
        }

        [Authorize]
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
                .Include(a => a.UserLikes)
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
        [Authorize(Roles = "Author,Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName");
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Author,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile ImageFile, [Bind("Id,Title,Body,CategoryId")] Article article)
        {
            if (ArticleExists(article.Title))
            {
                ModelState.AddModelError("Title", "The title already exists");
            }
            else if (ModelState.IsValid)
            {
                // If an image were sent to the server
                if (ImageFile != null)
                {
                    string imageName = "Article" + article.Id + System.IO.Path.GetExtension(ImageFile.FileName);
                    bool uploaded = await _service.UploadImage(ImageFile, imageName);
                    if (uploaded)
                    {
                        article.ImageLocation = _service.CLIENT_IMAGES_LOCATION + imageName;
                    }
                    else
                    {
                        ViewData["Error"] = "Can't upload this image, make sure its png,jpeg,jpg";
                        ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName", article.CategoryId);
                        return View(article);
                    }
                }
                else
                {
                    article.ImageLocation = DEFAULT_IMAGE;
                }
                article.CreationTimestamp = DateTime.Now;
                article.UserId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = article.Id });
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName", article.CategoryId);
            return View(article);
        }

        // GET: Articles/Edit/5
        [Authorize(Roles = "Author,Admin")]
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
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Author,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile ImageFile, [Bind("Id,Title,Body,ImageLocation,CategoryId")] Article newArticle)
        {
            var currArticle = await _context.Article.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            newArticle.ImageLocation = currArticle.ImageLocation;
            newArticle.CreationTimestamp = currArticle.CreationTimestamp;
            newArticle.UserId = currArticle.UserId;
            if ((currArticle == null) || (id != newArticle.Id))
            {
                return NotFound();
            }

            bool NotDuplicatedTitle = true;
            if (currArticle.Title != newArticle.Title)
            {
                NotDuplicatedTitle = !ArticleExists(newArticle.Title);
            }

            if (!NotDuplicatedTitle)
            {
                ModelState.AddModelError("Title", "The title already exists");
            } 
            else if (ModelState.IsValid)
            {
                if (ImageFile != null)
                {
                    string imageName = "Article" + newArticle.Id + System.IO.Path.GetExtension(ImageFile.FileName);
                    bool uploaded = await _service.UploadImage(ImageFile, imageName);
                    if (uploaded)
                    {
                        newArticle.ImageLocation = _service.CLIENT_IMAGES_LOCATION + imageName;

                        // Delete the old image if the name was changed, for exmaple: Article2.png changed to Article2.jpg
                        if (DEFAULT_IMAGE != currArticle.ImageLocation && newArticle.ImageLocation != currArticle.ImageLocation)
                        {
                            _service.DeleteImage(System.IO.Path.GetFileName(currArticle.ImageLocation));
                        }
                    }
                    else
                    {
                        ViewData["Error"] = "Can't upload this image, make sure its png,jpeg,jpg";
                        ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName", newArticle.CategoryId);
                        return View(newArticle);
                    }
                }
                try
                {
                    _context.Update(newArticle);
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
                return RedirectToAction(nameof(Details), new { id = newArticle.Id });
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "CategoryName", newArticle.CategoryId);
            return View(newArticle);
        }

        // POST: Articles/Delete/5
        [Authorize(Roles = "Author,Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Article.FindAsync(id);
            if (DEFAULT_IMAGE != article.ImageLocation)
            {
                _service.DeleteImage(System.IO.Path.GetFileName(article.ImageLocation));
            }
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
            return Json(await _context.Article.Where(a => (a.Title.Contains(queryTitle))).OrderByDescending(a => a.CreationTimestamp).ToListAsync());
        }

        public async Task<IActionResult> AdvancedSearch(int? categoryId, string author, string title)
        {
            if(categoryId == -1)
            {
                categoryId = null;
            }
            return Json(await _context.Article.Include(a => a.User).Where(a => (a.CategoryId.Equals(categoryId) || categoryId == null) && 
            (a.Title.Contains(title) || title == null) && 
            ((a.User.Firstname + " " + a.User.Lastname).Contains(author) || author == null)).OrderByDescending(a => a.CreationTimestamp).Select(a => new
            {
                a.Id,
                a.Title,
                a.ImageLocation,
                a.CreationTimestamp,
                a.Body
            }).ToListAsync());
        }

        public async Task<IActionResult> GetAll()
        {
            var dGNContext = _context.Article.Include(a => a.Category).Include(a => a.User).OrderByDescending(a => a.CreationTimestamp);
            return View(await dGNContext.ToListAsync());
        }

        public async Task<IActionResult> GetMostCommentedArticles(int count)
        {
            var query = from comment in _context.Comment
                        join article in _context.Article on comment.RelatedArticleId equals article.Id
                        group comment by new { article.Id, article.Title, article.ImageLocation } into ArticleCommentsGroup
                        orderby ArticleCommentsGroup.Count() descending
                        select ArticleCommentsGroup.Key;

            return Json(await query.Take(count).ToListAsync());
        }

        public async Task<IActionResult> GetMostLikedArticles(int count)
        {
            return Json(await _context.Article.Include(a => a.UserLikes).OrderByDescending(a => a.UserLikes.Count()).Take(count).Select(a => new
            {
                a.Id,
                a.ImageLocation,
                a.Title
            }).ToListAsync());
        }

        [Authorize]
        public async Task<IActionResult> LikeArticle(int? articleId)
        {
            if (articleId == null)
            {
                return NotFound();
            }

            var article = await _context.Article.Include(a => a.UserLikes).FirstOrDefaultAsync(a => a.Id == articleId);
            if (article == null)
            {
                return NotFound();
            }

            // Get the user from the UserId
            int userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.User.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);

            var isLiked = article.UserLikes.Any(ul => ul.Id == userId);

            if (isLiked)
            {
                article.UserLikes.Remove(article.UserLikes.Where(u => u.Id == userId).FirstOrDefault());
            } else {
                article.UserLikes.Add(user);
            }

            _context.Update(article);
            await _context.SaveChangesAsync();

            return Json(new { isLiked = !isLiked, count = article.UserLikes.Count()});
        }

        public async Task<IActionResult> GetArticlesByCategory()
        {
            var query = from article in _context.Article
                        join category in _context.Category on article.CategoryId equals category.Id
                        group article by new { category.Id, category.CategoryName } into ArticleCategoryGroup
                        orderby ArticleCategoryGroup.Count() descending
                        select new { CategoryId = ArticleCategoryGroup.Key.Id, CategoryName = ArticleCategoryGroup.Key.CategoryName, ArticlesInCategory = ArticleCategoryGroup.Count() };

            return Json(await query.ToListAsync());
        }

        public async Task<IActionResult> GetArticlesDates()
        {
            var query = from article in _context.Article
                        orderby article.CreationTimestamp
                        select article.CreationTimestamp;

            return Json(await query.ToListAsync());
        }
    }
}