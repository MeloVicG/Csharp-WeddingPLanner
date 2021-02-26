using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using WeddingPlanner.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyContext _context;

        public HomeController(MyContext Context)
        {
            _context = Context;
        }
    //-------------------------------------------------------
    //-------------------------------------------------------

    //-------------------------------------------------------
        //get current user?
        //why did we create this? //to replace with check user in dashboard
        //how are we using it?
        public User GetCurrentUser()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");

        if(userId == null)
        {
            return null;
        }
        return _context.Users
        .First(u=>u.UserId==userId);
    }

    //-------------------------------------------------------
        [HttpGet("")]
        public IActionResult Index()
        {

            return View();
        }

    //-------------------------------------------------------
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            // instead of using this to check for logged in user GetCurrentUser()
            // //get logged user info from session
            // int? userId = HttpContext.Session.GetInt32("UserId");

            // //check to ensure logged in (not to be able to access dashboard unless logged in)
            // if(userId == null)
            // {
            //     return RedirectToAction("Index");
            // }

            var currentUser = GetCurrentUser();
            if(currentUser == null)
            {
                return RedirectToAction("Index");
            }

            // pass user info to the view??
            ViewBag.CurrentUser = currentUser; 
            // _context.Users.First(u => u.UserId == userId);

            ViewBag.Weddings = _context.Weddings
            .Include(w=>w.PlannedBy)
            .Include(w=>w.Guests)
            .ToList();
            //order by descending
            // .OrderByDescending(m => m.blank.blank.Count)

            


            return View();
        }

    //-------------------------------------------------------
        [HttpPost("rsvp/{Wedid}")]
        public IActionResult RSVP(int Wedid)
        {
            var newRsvp = new RSVP();

            //have to match user with wedding ID connected to this user / Wedding
            newRsvp.UserId = HttpContext.Session.GetInt32("UserId").GetValueOrDefault();
            newRsvp.WeddingId = Wedid; //its a model because of line 61
            //look over this more

            _context.Add(newRsvp);
            _context.SaveChanges();
            return RedirectToAction("DashBoard");
            
        }
    //-------------------------------------------------------
        [HttpPost("unrsvp/{Wedid}")]
        public IActionResult UNRSVP(int Wedid)
        {
            
            var currentUser = GetCurrentUser();

            //get the RSVP!
            var RsvpDelete = _context.RSVP
                .First(r => r.WeddingId == Wedid && r.UserId == currentUser.UserId);

            _context.Remove(RsvpDelete);
            _context.SaveChanges();

            return RedirectToAction("DashBoard");
        }
    //-------------------------------------------------------
        [HttpPost("delete/wedding/{Wedid}")]
        public IActionResult DeleteWedding(int Wedid)
        {
            var WeddingToDelete = _context.Weddings
                .First(w=>w.WeddingId == Wedid);

            _context.Remove(WeddingToDelete);
            _context.SaveChanges();

            return RedirectToAction("DashBoard");
        }
    //-------------------------------------------------------
        [HttpGet("plan-wedding")]
        public IActionResult PlanWedding()
        {
            return View();
        }
    //-------------------------------------------------------
    //single wedding page
    //work on understanding this
        [HttpGet("wedding-detail/{wedId}")]
        public IActionResult Detail(int wedId)
        {
            ViewBag.PlannedEvent = _context.Weddings
            .Include(w => w.PlannedBy)  //this throws me off... i dont think i need it??
            .Include(w => w.Guests) //for each wedding i want to include Guests //gives us access to the guests at wedding
                .ThenInclude(r => r.User) //for each rsvp give back the User who RSVP
            .First(w => w.WeddingId == wedId);

            return View();
        }
    //-------------------------------------------------------
        [HttpPost("create-wedding-process")]
        public IActionResult CreateWedding(Wedding newWedding ) //Wedding weddingId will it work?
        {
            //validating must be future date 
            if(newWedding.Date < DateTime.Now)
                {
                    ModelState.AddModelError("Date", "Please select a future Date");
                    // return View("plan-wedding", newWedding);
                }
            // ViewBag.PlannedEvent = _context.Weddings
            if(ModelState.IsValid)
            {
                newWedding.UserId = HttpContext.Session.GetInt32("UserId").GetValueOrDefault();
                //insert wedding into DB

                _context.Add(newWedding);
                _context.SaveChanges();

                // HttpContext.Session.SetInt32("UserId", newWedding.UserId);

                return Redirect($"wedding-detail/{newWedding.WeddingId}");
            }

            return View("PlanWedding");
        }
    //-------------------------------------------------------
        [HttpPost("register-process")]
        public IActionResult RegisterProcess(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(_context.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("index", newUser);
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                //insert user into DB
                _context.Add(newUser);
                _context.SaveChanges();

                HttpContext.Session.SetInt32("UserId", newUser.UserId);

                return RedirectToAction("dashboard");
            }
            return View("Index");
        }
    //-------------------------------------------------------
        [HttpPost("login-process")]
        public IActionResult LoginProcess(LoginUser userSubmission)
        {
            if(ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = _context.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                // If no user exists with provided email
                if(userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }
                
                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();
                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
                
                // result can be compared to 0 for failure
                if(result == 0)
                {
                    // handle failure (this should be similar to how "existing email" is handled)
                    ModelState.AddModelError("Password", "Password Invalid");
                    return View("Index");
                }
                    HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                    return RedirectToAction("Dashboard", userSubmission);
            }
            return RedirectToAction("Dashboard");
        }
    //-------------------------------------------------------
        [HttpGet("logout")]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index");
        }
    //-------------------------------------------------------
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
