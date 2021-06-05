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

        // GET: Users/Details/5
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
            if (_context.User.Where<User>(u => u.Email == Email).ToList<User>().Count != 1)
            {
                ViewData["Error"] = "The email or password is incorrect";
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else {
                User usr = _context.User.Where(u => u.Email == Email).FirstOrDefault();
                Password pwd = _context.Password.Where(p => p.UserId == usr.Id).FirstOrDefault();
                if (pwd.Check(password))
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
            if (!ValidPass(plainPass))
            {
                ViewData["Error"] = "The minumum requierments are: 8 characters long containing 1 uppercase letter, 1 lowercase letter, a number and a special character";
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            if (!plainPass.Equals(confirmPass)) 
            {
                ViewData["Error"] = "Passwords does not match";
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            if (_context.User.Where<User>(u => u.Username == user.Username).ToList<User>().Count != 0)
            {
                ModelState.AddModelError("Username", "Username is aleardy exists");
                Response.StatusCode = (int)HttpStatusCode.Conflict;
            }
            if (_context.User.Where<User>(u => u.Email == user.Email).ToList<User>().Count != 0)
            {
                ModelState.AddModelError("Email", "This mail address is aleady in use");
                Response.StatusCode = (int)HttpStatusCode.Conflict;
            }
            if (ModelState.IsValid)
            {
                Password userPass = new Password(user.Id, plainPass, user);
                user.Password = userPass;
                _context.Add(user);
                _context.Add(userPass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        private bool ValidPass(string plainPass)
        {
            Regex regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
            return regex.IsMatch(plainPass);
        }

        // GET: Users/ChangePassword/5
        [Authorize]
        public async Task<IActionResult> ChangePassword(int? id)
        {
            string usersid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string userRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            if (id == null)
            {
                return NotFound();
            }
            if ((!usersid.Equals(id.ToString())) && (!userRole.Equals(UserRole.Admin.ToString())))
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
        public async Task<IActionResult> ChangePassword(int id, string currentPassword, string newPassword, string confirmNewPassword)
        {
            if (id == null)
            {
                return NotFound();
            }
            // Getting current user info from cookies
            string usersid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string userRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            // If current user trying to replace password to diffrent user and he is not Admin
            if ((!usersid.Equals(id.ToString())) && (!userRole.Equals(UserRole.Admin.ToString())))
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Unauthorized();
            }

            // Getting the user and password info from db
            User user = _context.User.Find(id);
            Password password = _context.Password.Where<Password>(p => p.UserId == id).FirstOrDefault();

            // Checking password is correct
            if (!password.Check(currentPassword)) {
                ViewData["Error"] = "Password is not currect";
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View();
            }
            // Making sure new password is valid
            if (!ValidPass(newPassword))
            {
                ViewData["Error"] = "The minumum requierments are: 8 characters long containing 1 uppercase letter, 1 lowercase letter, a number and a special character";
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View();
            }
            // Making sure new password confirmed
            if (!newPassword.Equals(confirmNewPassword))
            {
                ViewData["Error"] = "Passwords does not match";
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View();
            }
            // Creating the new password
            Password p = new Password(id, newPassword, user);
            _context.Password.Remove(password);
            _context.Password.Add(p);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Users/Delete/5
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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
    }
}
