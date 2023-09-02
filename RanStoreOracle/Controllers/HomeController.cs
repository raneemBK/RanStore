using Microsoft.AspNetCore.Mvc;
using RanStoreOracle.Models;
using System.Diagnostics;

namespace RanStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ModelContext _context;

        public HomeController(ILogger<HomeController> logger,ModelContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["NumberOfCustomers"] = _context.Logins.Where(x => x.RoleId == 1).Count();
            ViewData["NumberOfItems"] = _context.Items.Count();
            ViewData["numberOfOrder"] = _context.Carts.Where(s => s.State == "Finished").Count();
            ViewData["NumberofTestimonial"] = _context.Testimonials.Count();
            var home = _context.Homes.ToList();
            var about = _context.Abouts.ToList();
            var category = _context.Categories.ToList();
            var testimonial = _context.Testimonials.ToList();
            var user = _context.Users.ToList();
            var model = from u in user
                        join t in testimonial
                        on u.Id equals t.UserId
                        select new ModelTestimoinal { Testimoinal = t, User = u };
            //var home = _context.Homes.ToList();
            var tuple = Tuple.Create< IEnumerable < Home > , IEnumerable< About > ,IEnumerable<Category>, IEnumerable<ModelTestimoinal>>(home,about,category, model);
            return View(tuple);
        }
        public IActionResult Privacy()
        {
            return View();
        } 
        public IActionResult Testimonial()
        {
            var testimonial = _context.Testimonials.Where(t => t.Status == "Accept").ToList();
            var user = _context.Users.ToList();
            var model = from u in user
                        join t in testimonial
                        on u.Id equals t.UserId
                        select new ModelTestimoinal { Testimoinal = t, User = u };
            var tuple = Tuple.Create< IEnumerable<ModelTestimoinal>>( model);
            return View(tuple);
        }
        public IActionResult About()
        {
            ViewData["NumberOfCustomers"] = _context.Logins.Where(x => x.RoleId == 1).Count();
            ViewData["NumberOfItems"] = _context.Items.Count();
            ViewData["numberOfOrder"] = _context.Carts.Where(s => s.State == "Finished").Count();
            ViewData["NumberofTestimonial"] = _context.Testimonials.Count();
            var about = _context.Abouts.ToList();
            return View(about);
        }
        public IActionResult Category()
        {
            var category = _context.Categories.ToList();
            return View(category);
        }
        public IActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact([Bind("Id,Fullname,Email,Subject,Message")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}