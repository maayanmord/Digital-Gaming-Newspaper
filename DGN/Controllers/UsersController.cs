using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using DGN.Data;
using DGN.Models;
using System.Text.RegularExpressions;
using System.Net;
using System.Collections.Generic;
using System.Security.Claims;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DGN.Controllers
{
    public class UsersController : Controller
    {
        private readonly DGNContext _context;

        public UsersController(DGNContext context)
        {
            _context = context;
        }

        // GET: Users
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.User.ToListAsync());
        }

        // GET: Users/Profile/5
        [Authorize]
        public async Task<IActionResult> Profile(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // GET: Users/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string Email, string password)
        {            
            if (!EmailExists(Email))
            {
                ViewData["Error"] = "The email or password is incorrect";
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else {
                User usr = await _context.User.Include(u => u.Password).FirstOrDefaultAsync(u => u.Email == Email);
                if (usr.Password.Check(password))
                {
                    List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, usr.Id.ToString()),
                        new Claim(ClaimTypes.Name, usr.Username),
                        new Claim(ClaimTypes.Email, usr.Email),
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
                    return Redirect("/");
                }
                else 
                {
                    ViewData["Error"] = "The email or password is incorrect";
                    Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
            }
            return View();
        }


        // GET: Users/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Users/Registar
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Id,Email,Username,Firstname,Lastname,Birthday,Role,ImageLocation,About")] User user, string plainPass, string confirmPass)
        {
            if (UsernameExists(user.Username))
            {
                ModelState.AddModelError("Username", "Username is aleardy exists");
                Response.StatusCode = (int)HttpStatusCode.Conflict;
            }
            if (EmailExists(user.Email))
            {
                ModelState.AddModelError("Email", "This Email address is aleady in use");
                Response.StatusCode = (int)HttpStatusCode.Conflict;
            }
            if (CanUsePassword(plainPass, confirmPass) && ModelState.IsValid)
            {
                user.Password = new Password(user.Id, plainPass, user);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/ChangePassword/5
        [Authorize]
        public async Task<IActionResult> ChangePassword(int? id)
        {
            string userSid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string userRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            if (id == null)
            {
                return NotFound();
            }
            // Only when the user tring to change his own password
            if (!userSid.Equals(id.ToString()))
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Unauthorized();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ChangePassword(int? id, string currentPassword, string newPassword, string confirmNewPassword)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Getting current user info from cookies
            string userSid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string userRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            // Only when the user tring to change his own password
            if (!userSid.Equals(id.ToString()))
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Unauthorized();
            }

            // Getting the user and password info from db
            User user = await _context.User.Include(u => u.Password).FirstOrDefaultAsync(u => u.Id == id);

            // Checking password is correct
            if (!user.Password.Check(currentPassword)) {
                ViewData["Error"] = "Password is not currect";
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View();
            }
            if (CanUsePassword(newPassword, confirmNewPassword)) {
                // Creating the new password
                user.Password = new Password(user.Id, newPassword, user);
                _context.User.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: Users/Delete/5
        // TODO: DELETE USER ONLY IF I AM CURRENT USER AND CONFIRMD PASSWORD
        // OR WHEN ADMIN WITHOUT CONFIRM - ADMIN PASS CONFIRMD - NEW FUNC
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        // TODO: DELETE USER ONLY IF I AM CURRENT USER AND CONFIRMD PASSWORD
        // OR WHEN ADMIN WITHOUT CONFIRM - ADMIN PASS CONFIRMD - NEW FUNC
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

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
            Regex regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
            return regex.IsMatch(plainPass);
        }

        private bool CanUsePassword(string password, string confirmPassword) {
            // Making sure new password is valid
            if (!ValidPass(password))
            {
                ViewData["Error"] = "The minumum requierments are: 8 characters long containing 1 uppercase letter, 1 lowercase letter, a number and a special character";
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return false;
            }
            // Making sure new password confirmed
            if (!password.Equals(confirmPassword))
            {
                ViewData["Error"] = "Passwords does not match";
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return false;
            }
            return true;
        }
    }
}
