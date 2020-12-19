using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WeddingPlanner.Contexts;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {

        private HomeContext _context;

        //Allows us to check if the user in session is in the database.
        private User GetUserFromDB()
        {
            return _context.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
        }

        public HomeController(HomeContext context)
        {
            _context = context;
        }

        //Returns the Homepage for wedding planner.
        public IActionResult Index()
        {
            return View();
        }


        //Sends Login info to be checked if person is in the system. Then adds them to session and redirects to Dashboard.
        [HttpPost("Login")]
        public IActionResult Login(LoginUser log)
        {
            if (ModelState.IsValid)
            {
                User userInDB = _context.Users.FirstOrDefault(u => u.Email == log.LoginEmail);
                if (userInDB == null)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                    ModelState.AddModelError("LoginPassword", "Invalid Email/Password");
                    return View("Index");
                }
                PasswordHasher<LoginUser> hash = new PasswordHasher<LoginUser>();
                var result = hash.VerifyHashedPassword(log, userInDB.Password, log.LoginPassword);

                if (result == 0)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                    ModelState.AddModelError("LoginPassword", "Invalid Email/Password");
                    return View("Index");
                }

                HttpContext.Session.SetInt32("UserId", userInDB.UserId);
                Console.WriteLine("Logged in User");
                return RedirectToAction("Dashboard");
            }
            return View("Index");
        }


        //Sends info to be checked if person is in the system. Then adds them to the database and puts them into session. Redirects to dashboard.
        [HttpPost("Register")]
        public IActionResult SignUp(User reg)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email == reg.Email))
                {
                    ModelState.AddModelError("Email", "That email is already taken.");
                    return View("Index");
                }
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                reg.Password = hasher.HashPassword(reg, reg.Password);
                _context.Users.Add(reg);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("UserId", reg.UserId);
                Console.WriteLine("Created User");
                return RedirectToAction("Dashboard");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            User userInDB = GetUserFromDB();
            if (userInDB == null)
            {
                return RedirectToAction("Logout");
            }
            ViewBag.User = userInDB;
            List<Wedding> AllWeddings = _context.Weddings
            .Include(p => p.Creator)
            .Include(p => p.Attendees)
            .ThenInclude(g => g.AGuest)
            .ToList();
            return View(AllWeddings);
        }


        [HttpGet("CreateWedding")]

        public IActionResult Wedding()
        {
            User userInDB = GetUserFromDB();
            if (userInDB != null)
            {
                return View();
            }
            return RedirectToAction("Logout");
        }

        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpPost("PushWedding")]

        public IActionResult PushWedding(Wedding wedding)
        {
            User userInDB = GetUserFromDB();

            if (userInDB != null)
            {
                if (ModelState.IsValid)
                {
                    wedding.UserId = userInDB.UserId;
                    _context.Weddings.Add(wedding);
                    _context.SaveChanges();
                    return RedirectToAction("Dashboard");
                }
                return View("CreateWedding");
            }
            return RedirectToAction("Logout");
        }

        [HttpGet("attend/{userId}/{weddingId}")]

        public IActionResult Attend(int userId, int weddingId)
        {
            Guest going = new Guest();
            going.UserId = userId;
            going.WeddingId = weddingId;
            _context.Guests.Add(going);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("ditch/{userId}/{weddingId}")]

        public IActionResult Ditch(int userId, int weddingId)
        {
            Guest ditching = _context.Guests.FirstOrDefault(r => r.UserId == userId && r.WeddingId == weddingId);
            _context.Guests.Remove(ditching);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("delete/{weddingId}")]

        public IActionResult Delete(int weddingId)
        {
            Wedding crasher = _context.Weddings.FirstOrDefault(c => c.WeddingId == weddingId);
            _context.Weddings.Remove(crasher);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("ShowWedding/{weddingId}")]

        public IActionResult ShowWedding(int weddingId)
        {
            Wedding single = _context.Weddings
            .Include(p => p.Attendees)
            .ThenInclude(r => r.AGuest)
            .Include(p => p.Creator)
            .FirstOrDefault(w => w.WeddingId == weddingId);

            User userInDB = GetUserFromDB();
            ViewBag.User = userInDB;

            return View(single);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
