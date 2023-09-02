using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RanStoreOracle.Models;
using System.IO;

namespace RanStore.Controllers
{
    public class LoginAndRegisterController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public LoginAndRegisterController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LogoutCustomer()
        {
            var _id1 = _context.Users.FindAsync(Convert.ToDecimal(HttpContext.Session.GetInt32("CustomerId")));

            if (_id1 != null)
            {
                HttpContext.Session.SetInt32("CustomerId", 0);
                HttpContext.Session.SetInt32("LoginId", 0);
                return RedirectToAction("Login");
            }
            return View();
        }
        public IActionResult LogoutAdmin()
        {
            var _id1 =_context.Users.FindAsync(Convert.ToDecimal(HttpContext.Session.GetInt32("AdminId")));

            if (_id1 != null)
            {
                HttpContext.Session.SetInt32("AdminId", 0);
                HttpContext.Session.SetInt32("LoginId", 0);
                return RedirectToAction("Login");
            }
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username,Password")] Login login)
        {
            var auth = _context.Logins.Where(x => x.Username == login.Username && x.Password == login.Password).SingleOrDefault();
            
            if (auth != null)
            {
                switch (auth.RoleId)
                {
                    case 1:
                        HttpContext.Session.SetInt32("CustomerId", (int)auth.UserId);
                        HttpContext.Session.SetInt32("LoginId",(int)auth.Id);
                        return RedirectToAction("Index", "User");
                    case 2:
                        //HttpContext.Session.SetInt32("AdminId", (int)auth.UserId);
                        
                        HttpContext.Session.SetInt32("AdminId", (int)auth.UserId);
                        HttpContext.Session.SetInt32("LoginId", (int)auth.Id);
                        var adminName = _context.Users.FirstOrDefault(u => u.Id == HttpContext.Session.GetInt32("AdminId"));
                        HttpContext.Session.SetString("AdminName", adminName.Fname);
                        HttpContext.Session.SetString("AdminImage", adminName.Imagepath);
                        return RedirectToAction("Index", "Admin");
                }
            }
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Id,Fname,Lname,ImageFile,Gender")] User user, string userName, string password, string type)
        {
            if (ModelState.IsValid || user.ImageFile != null)
            {
                //Add Customer Details
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + "_" + user.ImageFile.FileName;
                /*string extension = Path.GetExtension(customer.ImageFile.FileName);
                customer.ImagePath = fileName;*/
                string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await user.ImageFile.CopyToAsync(fileStream);
                }
                user.Imagepath = fileName;
                _context.Add(user);
                await _context.SaveChangesAsync();

                //Add Login Details
                Login login = new Login();
                login.Username = userName;
                login.Password = password;
                login.UserId = user.Id;
                if (type == "customer")
                {
                    login.RoleId = 1;
                }
                if (type == "admin")
                {
                    login.RoleId = 2;
                }
                _context.Add(login);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(user);
        }

        public IActionResult EditPass()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPass(int id, string oldPass, string newPass)
        {
            var _id = Convert.ToDecimal(HttpContext.Session.GetInt32("LoginId"));
            var profile = await _context.Logins.FindAsync(_id);
            if (profile == null)
            {
                return NotFound();
            }
            else
            {
               if (profile.UserId == id && profile.Password == oldPass)
                {
                    profile.Password = newPass;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Login");
                }
               

            }
            return View();
        }

        public IActionResult EditEmail()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmail(int id, string oldEmail, string newEmail)
        {
            var _id = Convert.ToDecimal(HttpContext.Session.GetInt32("LoginId"));
            var profile = await _context.Logins.FindAsync(_id);
            if (profile == null)
            {
                return NotFound();
            }
            else
            {
                if (profile.UserId == id && profile.Username == oldEmail)
                {
                    profile.Username = newEmail;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Login");
                }


            }
            return View();
        }

        public IActionResult EditPhoto()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPhoto(int id, IFormFile photo)
        {
            var _id = Convert.ToDecimal(id);
            var profile = await _context.Users.FindAsync(_id);
            var loginId = await _context.Logins.FindAsync(Convert.ToDecimal(HttpContext.Session.GetInt32("LoginId")));
            if (profile == null)
            {
                return NotFound();
            }
            else
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
             
                string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await photo.CopyToAsync(fileStream);
                }
                profile.Imagepath = fileName;
                _context.Update(profile);
                await _context.SaveChangesAsync();
                if (loginId.RoleId == 1)
                {
                    return RedirectToAction("Profile","User");
                }
                else
                {
                    return RedirectToAction("Profile","Admin");
                }
                
            }
            
        }
    }
}
