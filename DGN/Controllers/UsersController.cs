﻿using DGN.Data;
using DGN.Models;
using DGN.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DGN.Controllers
{
    public class UsersController : Controller
    {
        private readonly DGNContext _context;
        private readonly ImagesService _service;
        private string DEFAULT_IMAGE;

        public UsersController(DGNContext context, ImagesService service)
        {
            _context = context;
            _service = service;
            DEFAULT_IMAGE = _service.CLIENT_IMAGES_LOCATION + "DefaultProfileImage.jpg";
        }

        //
        //  The following function are for the Managers to manage the users
        //  From here Admins can change users and user roles and see all users
        //
         
        // GET Users
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.User.ToListAsync());
        }

        // GET: Users/EditAsAdmin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditAsAdmin(int? id)
        {
            ViewData["Roles"] = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Text = UserRole.Admin.ToString(), Value = UserRole.Admin.ToString()},
                    new SelectListItem { Text = UserRole.Author.ToString(), Value = UserRole.Author.ToString()},
                    new SelectListItem { Text = UserRole.Client.ToString(), Value = UserRole.Client.ToString()},
                }, "Text", "Value");
            return await GetUserView(id);
        }

        // POST: Users/EditAsAdmin
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsAdmin(int? id, IFormFile ImageFile, [Bind("Id,Email,Firstname,Lastname,Birthday,Role,About")] User user)
        {
            ViewData["Roles"] = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Text = UserRole.Admin.ToString(), Value = UserRole.Admin.ToString()},
                    new SelectListItem { Text = UserRole.Author.ToString(), Value = UserRole.Author.ToString()},
                    new SelectListItem { Text = UserRole.Client.ToString(), Value = UserRole.Client.ToString()},
                }, "Text", "Value");
            return await PostEditUser(id, ImageFile, user, true, RedirectToAction(nameof(Index)));
        }

        //
        // The following functions are for the Authentication
        //

        // GET: Users/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Users/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string confirmPass, IFormFile ImageFile, string plainPass, [Bind("Id,Email,Username,Firstname,Lastname,Birthday,About")] User user)
        {
            if (UsernameExists(user.Username))
            {
                ModelState.AddModelError("Username", "Username is aleardy exists");
            }
            if (EmailExists(user.Email))
            {
                ModelState.AddModelError("Email", "This Email address is aleady in use");
            }
            if (CanUsePassword(plainPass, confirmPass) && ModelState.IsValid)
            {
                user.Password = new Password(user.Id, plainPass, user);
                if (ImageFile != null)
                {
                    string imageName = "User" + user.Id + System.IO.Path.GetExtension(ImageFile.FileName);
                    bool uploaded = await _service.UploadImage(ImageFile, imageName);
                    if (uploaded)
                    {
                        user.ImageLocation = _service.CLIENT_IMAGES_LOCATION + imageName;
                    }
                    else
                    {
                        ViewData["Error"] = "Can't upload this image, make sure its png,jpeg,jpg";
                        return View(user);
                    }
                }
                else
                {
                    user.ImageLocation = DEFAULT_IMAGE;
                }
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Login));
            }
            return View(user);
        }

        // GET: /Users/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        // GET: Users/Login
        public IActionResult Login(string ReturnUrl)
        {
            ViewData["ReturnUrl"] = ReturnUrl;
            return View();
        }

        // POST: Users/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string Email, string password, string ReturnUrl)
        {
            if (!EmailExists(Email))
            {
                ViewData["Error"] = "The email or password is incorrect";
            }
            else
            {
                User usr = await _context.User.Include(u => u.Password).FirstOrDefaultAsync(u => u.Email == Email);
                if (usr.Password.Check(password))
                {
                    // Cookies that are encrypted
                    List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, usr.Id.ToString()),
                        new Claim(ClaimTypes.Name, usr.Username),
                        new Claim(ClaimTypes.Role, usr.Role.ToString())
                    };

                    ClaimsIdentity claimIdentity = new ClaimsIdentity(claims, "Login");
                    AuthenticationProperties authProperties = new AuthenticationProperties
                    {
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimIdentity),
                        authProperties);

                    if (Url.IsLocalUrl(ReturnUrl))
                        return Redirect(ReturnUrl);
                    else
                        return RedirectToAction(nameof(Index), "Articles");
                }
                else
                {
                    ViewData["Error"] = "The email or password is incorrect";
                }
            }
            ViewData["ReturnUrl"] = ReturnUrl;
            return View();
        }

        //
        // The following functions are for changing user password
        //

        // GET: Users/ChangePassword/5
        [Authorize]
        public async Task<IActionResult> ChangePassword(int? id)
        {
            if (!isAuthorizeEditor(id))
            {
                return Unauthorized();
            }
            return await GetUserView(id);
        }

        // POST: Users/ChangePassword/5
        // Leting users to change their own passwords
        // And admins change all users passwords
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ChangePassword(int? id, string currentPassword, string newPassword, string confirmNewPassword)
        {

            if (id == null)
            {
                return NotFound();
            }

            User user = await _context.User.Include(u => u.Password).FirstOrDefaultAsync(u => u.Id == id);

            if (!isAuthorizeEditor(id))
            {
                return Unauthorized();
            }
            else
            {
                if (currentPassword == null)
                {
                    ViewData["Error"] = "You must specify current password";
                    return View();
                }
                if (!user.Password.Check(currentPassword))
                {
                    ViewData["Error"] = "Password is not correct";
                    return View();
                }
            }

            if (CanUsePassword(newPassword, confirmNewPassword))
            {
                // Creating the new password
                Password newPass = new Password(user.Id, newPassword, user);
                user.Password.Hash = newPass.Hash;
                user.Password.Salt = newPass.Salt;
                _context.User.Update(user);
                await _context.SaveChangesAsync();
                return await Logout();
            }
            return View();
        }

        //
        // The following functions are for managing the users profiles
        //

        // GET: Users/Profile/5
        [Authorize]
        public async Task<IActionResult> Profile(int? id)
        {
            return await GetUserView(id);
        }

        // POST: Users/Profile/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(int? id, IFormFile ImageFile, [Bind("Id,Email,Firstname,Lastname,Birthday,Role,About")] User user)
        {
            if (!isAuthorizeEditor(id))
            {
                return Unauthorized();
            }
            return await PostEditUser(id, ImageFile, user, false, View(user));
        }

        //
        // The following functions are for deleting users
        //

        // GET: Users/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!isAuthorizeEditor(id))
            {
                return Unauthorized();
            }

            return await GetUserView(id);
        }

        // POST: Users/Delete/5
        // Allowing delete when
        // 1. user deletes itself
        // 2. Admin delete not admin user
        // in both cases the user needs to confirm its password
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id, string plainPass)
        {

            string userRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            string userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool logout = false;

            User user = await _context.User.Include(u => u.Password).Include(u => u.Comments).Include(u => u.Articles).FirstOrDefaultAsync(u => u.Id == id);

            if (plainPass == null)
            {
                ViewData["Error"] = "Your password is required";
                return View(user);
            }
            IActionResult redirect;
            if (!userId.Equals(id.ToString()))
            {
                redirect = RedirectToAction(nameof(Index));
                if (userRole.Equals(UserRole.Admin.ToString()))
                {
                    User currentUser = await _context.User.Include(u => u.Password).FirstOrDefaultAsync(u => u.Id.ToString() == userId);
                    if (!currentUser.Password.Check(plainPass))
                    {
                        ViewData["Error"] = "Password is not correct";
                        return View(user);
                    }
                    if (user.Role == UserRole.Admin)
                    {
                        ViewData["Error"] = "Can not delete admin user";
                        return View(user);
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                redirect = Redirect("/");
                if (!user.Password.Check(plainPass))
                {
                    ViewData["Error"] = "password is not correct";
                    return View(user);
                }
                logout = true;
            }
            if (DEFAULT_IMAGE != user.ImageLocation)
            {
                _service.DeleteImage(System.IO.Path.GetFileName(user.ImageLocation));
            }
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            if (logout)
            {
                await Logout();
            }
            return redirect;
        }

        //
        // The following functions are for searching and getting partial data
        // 

        [Authorize]
        public async Task<IActionResult> GetUserLikedArticles(int id, int page, int count)
        {
            var query = from article in _context.Article
                        where article.UserLikes.Any(u => u.Id == id)
                        select article;
            return Json(await query.Skip(page * count).Take(count).ToListAsync());
        }

        [Authorize]
        public async Task<IActionResult> GetUserCommentedArticles(int? id, int page, int count)
        {
            var query = from comment in _context.Comment
                        join article in _context.Article on comment.RelatedArticleId equals article.Id
                        where comment.UserId == id
                        select article;

            return Json(await query.Distinct().Skip(page*count).Take(count).ToListAsync());
        }

        [Authorize]
        public async Task<IActionResult> GetUserArticles(int? id, int page, int count)
        {
            return Json(await _context.Article.Where(a => a.UserId == id).Skip(page * count).Take(count).ToListAsync());
        }

        //
        // The following functions are private helpful functions
        //

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }

        private bool EmailExists(string Email)
        {
            return _context.User.Any(e => e.Email == Email);
        }

        private bool UsernameExists(string Username)
        {
            return _context.User.Any(e => e.Username == Username);
        }
        private bool ValidPass(string plainPass)
        {
            Regex regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])\S{8,}$");
            return regex.IsMatch(plainPass);
        }

        // Checking if the given input is valid to be set as password
        private bool CanUsePassword(string password, string confirmPassword)
        {
            if (confirmPassword == null || password == null)
            {
                ViewData["Error"] = "Make sure to fill up the fields Password and Confirm Password";
                return false;
            }
            // Making sure new password is valid and not null
            if (!ValidPass(password))
            {
                ViewData["Error"] = "The minumum requierments for password are: 8 characters long containing 1 uppercase letter, 1 lowercase letter, a number and a special character";
                return false;
            }
            // Making sure new password confirmed
            if (!password.Equals(confirmPassword))
            {
                ViewData["Error"] = "Passwords does not match";
                return false;
            }
            return true;
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
        private async Task<IActionResult> GetUserView(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            User user = await _context.User
                .Include(u => u.ArticleLikes)
                .Include(u => u.Articles)
                .Include(u => u.Comments)
                .ThenInclude(c => c.RelatedArticle)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        private async Task<IActionResult> PostEditUser(int? id, IFormFile ImageFile, User user, bool RoleChanged, IActionResult redirectPage)
        {
            bool logout = false;
            string userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (id == null || user == null || id != user.Id)
            {
                return NotFound();
            }

            User oldUser = await _context.User.AsNoTracking()
                .Include(u => u.ArticleLikes)
                .Include(u => u.Articles)
                .Include(u => u.Comments)
                .ThenInclude(c => c.RelatedArticle)
                .FirstOrDefaultAsync(m => m.Id == id);
            user.Username = oldUser.Username;
            user.ImageLocation = oldUser.ImageLocation;
            if (!RoleChanged)
            {
                user.Role = oldUser.Role;
                user.Email = oldUser.Email;
            }
            else if (oldUser.Role == UserRole.Admin && user.Role != UserRole.Admin) 
            {
                if (userId.Equals(user.Id.ToString()))
                {
                    logout = true;
                }
                else {
                    ModelState.AddModelError("Role", "Can't change role for Admin");
                }
            }
            if (oldUser.Email != user.Email)
            {
                if (EmailExists(user.Email))
                {
                    ModelState.AddModelError("Email", "The email already exists");
                }
            }
            ModelState.Remove("Username");
            if (ModelState.IsValid)
            {
                if (ImageFile != null)
                {
                    string fileName = "User" + user.Id + System.IO.Path.GetExtension(ImageFile.FileName);
                    bool uploaded = await _service.UploadImage(ImageFile, fileName);
                    if (uploaded)
                    {
                        user.ImageLocation = _service.CLIENT_IMAGES_LOCATION + fileName;
                        if (DEFAULT_IMAGE != oldUser.ImageLocation && user.ImageLocation != oldUser.ImageLocation)
                        {
                            _service.DeleteImage(System.IO.Path.GetFileName(oldUser.ImageLocation));
                        }
                    }
                    else
                    {
                        ViewData["Error"] = "Can't upload this image, make sure its png,jpeg,jpg";
                    }
                }
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    user.ArticleLikes = oldUser.ArticleLikes;
                    user.Articles = oldUser.Articles;
                    user.Comments = oldUser.Comments;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (logout) {
                    return await Logout();
                }
                return redirectPage;
            }

            user.ArticleLikes = oldUser.ArticleLikes;
            user.Articles = oldUser.Articles;
            user.Comments = oldUser.Comments;
            
            return View(user);
        }

        // Checking if the user tring to edit the id is the user itself or admin if so he is authorize, if not he is not
        private bool isAuthorizeEditor(int? id)
        {
            string userRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            string userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId.Equals(id.ToString()) || userRole.Equals(UserRole.Admin.ToString());
        }


        [Authorize(Roles = "Author,Admin")]
        // GET: Users/Statistics
        public IActionResult Statistics()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Search(string query)
        {
            string userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Json(await _context.User.Where(a => (a.Firstname.Contains(query) || a.Lastname.Contains(query) || a.Username.Contains(query) || a.Email.Contains(query) || query == null)).Select(a => new
            {
                a.Id,
                a.Email,
                a.Username,
                a.Firstname,
                a.Lastname,
                birthday = a.Birthday.ToString("dd-MM-yyyy"),
                role = a.Role.ToString(),
                canDelete = (a.Role.ToString() != UserRole.Admin.ToString() || userId.Equals(a.Id.ToString()))
            }
            ).ToListAsync()); ;
        }
    }
}
