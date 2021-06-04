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
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _context.User.ToListAsync());
        }

        // GET: Users/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
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

        // GET: Users/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email")] string Email, string password)
        {
            if (_context.User.Where<User>(u => u.Email == Email).ToList<User>().Count != 1)
            {
                ModelState.AddModelError("Password", "The email or password is incorrect");
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else {
                User usr = _context.User.Where(u => u.Email == Email).ToList<User>()[0];
                Password pwd = _context.Password.Where(p => p.Id == usr.Id).ToList<Password>()[0];
                if (pwd.Check(password))
                {
                    List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usr.Username),
                        new Claim(ClaimTypes.Email, usr.Email),
                        //new Claim(ClaimTypes.Role, "Admin")
                    };

                    ClaimsIdentity claimIdentity = new ClaimsIdentity(claims, "Login");
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity));
                    return Redirect("/");
                }
                else 
                {
                    ModelState.AddModelError("Password", "The email or password is incorrect");
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
                ModelState.AddModelError("Password", "The minumum requierments are: 8 characters long containing 1 uppercase letter, 1 lowercase letter, a number and a special character");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            if (!plainPass.Equals(confirmPass)) 
            {
                ModelState.AddModelError("Password", "Passwords does not match");
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

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Username,Firstname,Lastname,Birthday,Role,ImageLocation,About")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
            }
            return View(user);
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
