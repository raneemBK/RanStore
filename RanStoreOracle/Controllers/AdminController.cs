using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RanStoreOracle.Models;

namespace RanStore.Controllers
{
    public class AdminController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            ViewData["NumberOfCustomers"] = _context.Logins.Where(x => x.RoleId == 1).Count();
            ViewData["NumberOfItems"] = _context.Items.Count();
            /*var offers = _context.Offers.ToList();
            var numberOffers = from offer in offers
                               group offer by offer.ItemId;
            numberOffers = numberOffers.ToList();
            ViewBag.NumberOfOffer = numberOffers.Select(x=> x.Count());*/
            var itemOffersCount = _context.Items.Select(item => new
            {
                ItemId = item.Id,
                OffersCount = _context.Offers.Where(offer => offer.ItemId == item.Id).Count()
            }).ToList();
            ViewBag.ItemOffersCount = itemOffersCount;
            var salesData = _context.Sales
              .GroupBy(s => new { Year = s.Dateofsale.Value.Year, Month = s.Dateofsale.Value.Month })
              .Select(g => new { Year = g.Key.Year, Month = g.Key.Month, TotalAmount = g.Sum(s => s.Quantity) })
              .OrderBy(g => g.Year).ThenBy(g => g.Month).ToList();

            ViewBag.SalesData = salesData;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditImageAbout(IFormFile photo)
        {
            var _id = Convert.ToDecimal(5);
            var profile = await _context.Abouts.FindAsync(_id);
            //var loginId = await _context.Abouts.FindAsync(Convert.ToDecimal(HttpContext.Session.GetInt32("LoginId")));
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
                profile.Images = fileName;
                _context.Update(profile);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");

            }

        }
        public IActionResult EditAbout()
        {
            return View();
        }
        public IActionResult EditHome()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditImageHome(IFormFile photo)
        {
            var _id = Convert.ToDecimal(1);
            var profile = await _context.Homes.FindAsync(_id);
            //var loginId = await _context.Abouts.FindAsync(Convert.ToDecimal(HttpContext.Session.GetInt32("LoginId")));
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
                profile.Images = fileName;
                _context.Update(profile);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");

            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditParagraphHome(string paragraph)
        {
            var _id = Convert.ToDecimal(1);
            var profile = await _context.Homes.FindAsync(_id);
            if (profile == null)
            {
                return NotFound();
            }
            else
            {
                profile.Paragraph = paragraph;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");

            }
           
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditParagraphAbout(string paragraph)
        {
            var _id = Convert.ToDecimal(5);
            var profile = await _context.Homes.FindAsync(_id);
            if (profile == null)
            {
                return NotFound();
            }
            else
            {
                profile.Paragraph = paragraph;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");

            }

        }
        public async Task<IActionResult> ReadMail(decimal? id)
        {
            var result = await _context.Contacts.FirstOrDefaultAsync(x => x.Id == id);
            return View(result);
        }
        [HttpPost]
        public IActionResult ChickReport(string reportType)
        {
            if (reportType == "annual")
            {
                var salesData = _context.Sales.Where(s => s.Dateofsale.Value.Year == DateTime.Now.Year).ToList();

                return RedirectToAction("AnnualReport");
            }
            else if (reportType == "monthly")
            {
                var salesData = _context.Sales.Where(s => s.Dateofsale.Value.Month == DateTime.Now.Month).ToList();

                return RedirectToAction("MonthlyReport");
            }
            else
            {
                return RedirectToAction("Report");
            }
        }
        public IActionResult Report()
        {
            var report = _context.Sales.ToList();
            var item = _context.Items.ToList();
            var user = _context.Users.ToList();
            var sales = from r in report
                        join i in item
                        on r.Itemid equals i.Id
                        join u in user
                        on i.UserId equals u.Id
                        select new SalesItems { Sale = r , Item = i , User = u};
            var modelContext = _context.Sales.Include(c => c.User).Include(p => p.Item).ToList();
            ViewBag.TotalQuantity = modelContext.Sum(x => x.Quantity);
            ViewBag.TotalPrice = modelContext.Sum(x => x.Totalprice);
            
            var model3 = Tuple.Create<IEnumerable<SalesItems>, IEnumerable<Sale>>(sales, modelContext);
            return View(model3);
        }
        public IActionResult AnnualReport()
        {
            var report = _context.Sales.ToList();
            var item = _context.Items.ToList();
            var user = _context.Users.ToList();
            var sales = from r in report
                        join i in item
                        on r.Itemid equals i.Id
                        join u in user
                        on i.UserId equals u.Id
                        where r.Dateofsale.Value.Year == DateTime.Now.Year
                        select new SalesItems { Sale = r, Item = i, User = u };
            var modelContext = _context.Sales.Include(c => c.User).Include(p => p.Item).ToList();
            ViewBag.TotalQuantity = modelContext.Sum(x => x.Quantity);
            ViewBag.TotalPrice = modelContext.Sum(x => x.Totalprice);

            var model3 = Tuple.Create<IEnumerable<SalesItems>, IEnumerable<Sale>>(sales, modelContext);
            return View(model3);
        }
        public IActionResult MonthlyReport()
        {
            var report = _context.Sales.ToList();
            var item = _context.Items.ToList();
            var user = _context.Users.ToList();
            var sales = from r in report
                        join i in item
                        on r.Itemid equals i.Id
                        join u in user
                        on i.UserId equals u.Id
                        where r.Dateofsale.Value.Month == DateTime.Now.Month
                        select new SalesItems { Sale = r, Item = i, User = u };
            var modelContext = _context.Sales.Include(c => c.User).Include(p => p.Item).ToList();
            ViewBag.TotalQuantity = modelContext.Sum(x => x.Quantity);
            ViewBag.TotalPrice = modelContext.Sum(x => x.Totalprice);

            var model3 = Tuple.Create<IEnumerable<SalesItems>, IEnumerable<Sale>>(sales, modelContext);
            return View(model3);
        }
        [HttpPost]
        public async Task<IActionResult> Search(DateTime? startDate, DateTime? endDate)
        {
            var modelContext = _context.Sales.Include(c => c.User).Include(p => p.Item);
            if (startDate == null && endDate == null)
            {
                return View(modelContext);
            }
            else if (startDate == null && endDate != null)
            {
                // [StartDate (DateFrom), EndDate (DateFrom)]
                var result = await modelContext.Where(x => x.Dateofsale.Value.Date <= endDate).ToListAsync();
                return View(result);
            }
            else if (startDate != null && endDate == null)
            {
                var result = await modelContext.Where(x => x.Dateofsale.Value.Date >= startDate).ToListAsync();
                return View(result);
            }
            else
            {
                var result = await modelContext.Where(x => x.Dateofsale >= startDate && x.Dateofsale <= endDate).ToListAsync();
                return View(result);
            }
            return View();
        }
        public IActionResult Profile()
        {
            var profile = _context.Users.Where(x => x.Id == HttpContext.Session.GetInt32("AdminId"));
            return View(profile);
        }
        public IActionResult RegisterUser()
        {
            var user = _context.Users.ToList();
            var login = _context.Logins.ToList();
            var role = _context.Roles.ToList();
            var users = from c in user
                        join l in login on c.Id equals l.UserId
                        join r in role on l.RoleId equals r.Id
                        where r.Id == 1
                        select c;
            return View(users);
        }
        public IActionResult ContactUs()
        {
            var contact = _context.Contacts.ToList();
            return View(contact);
        }
        public IActionResult AcceptItem()
        {
            var item = _context.Items.Where(x=> x.Status == "NotAccept").ToList();
            return View(item);
        }
        public IActionResult AcceptOffer()
        {
            var item = _context.Offers.Where(x=> x.Status == "NotAccept").ToList();
            return View(item);
        }
        public IActionResult AcceptTest()
        {
            var item = _context.Testimonials.Where(x=> x.Status == "NotAccept").ToList();
            return View(item);
        }
        [HttpGet]
        public IActionResult Offer()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Offer(decimal id, string Status)
        {
            var item = await _context.Offers.FindAsync(id);
            
            if (item == null)
            {
                return NotFound();
            }
            else
            {
                item.Status = Status;
                await _context.SaveChangesAsync();

                var itemID = await _context.Items.FirstOrDefaultAsync(x => x.Id == item.ItemId);
                itemID.Price = item.Price;
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Admin");
            }

        }


        [HttpGet]
        public IActionResult Accept()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Accept(decimal id , string Status)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            else
            {
                item.Status = Status;
                await _context.SaveChangesAsync();
                return RedirectToAction("AcceptItem", "Admin");
            }
            
        }
        [HttpGet]
        public IActionResult Testmonial()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Testmonial(decimal id, string Status)
        {
            var item = await _context.Testimonials.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            else
            {
                item.Status = Status;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Admin");
            }

        }
               
    }
}
