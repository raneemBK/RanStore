using Aspose.Pdf;
using Aspose.Pdf.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RanStoreOracle.Models;
using System;
using System.Net;
using System.Net.Mail;

namespace RanStore.Controllers
{
    public class UserController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _environment;
        public UserController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _environment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var item = _context.Items.Where(x => x.Status == "Accept").ToList();
            var category = _context.Categories.ToList();
            var testimonial = _context.Testimonials.ToList();
            var user = _context.Users.ToList();
            var model = from u in user
                        join t in testimonial
                        on u.Id equals t.UserId
                        select new ModelTestimoinal { Testimoinal = t , User = u};
            
            var home = _context.Homes.ToList();
            var about = _context.Abouts.ToList();
            var tuple = Tuple.Create<IEnumerable<Item>, IEnumerable<Category>, IEnumerable<ModelTestimoinal>, IEnumerable<Home>, IEnumerable<About>>(item, category, model,home, about); ;
            return View(tuple);
        }
        public IActionResult About()
        {
            var about = _context.Abouts.ToList();
            return View(about);
        }
        public IActionResult Category()
        {
            var category = _context.Categories.ToList();
            return View(category);
        }
        public IActionResult Testimonial()
        {
            var testimonial = _context.Testimonials.ToList();
            var user = _context.Users.ToList();
            var model = from u in user
                        join t in testimonial
                        on u.Id equals t.UserId
                        select new ModelTestimoinal { Testimoinal = t, User = u };
            var tuple = Tuple.Create<IEnumerable<ModelTestimoinal>>(model);
            return View(tuple);
        }
        public IActionResult DisplayItems(decimal id)
        {
            var items = _context.Items.Where(x=> x.CategoryId == id).ToList();
            return View(items);
        }
        [HttpPost]
        public async Task<IActionResult> SearchItem(string search)
        {
            var model = from item in _context.Items
                        where item.Status == "Accept"
                        select item;
            if (!String.IsNullOrEmpty(search))
            {
                model = model.Where(s => s.Name!.Contains(search));
            }
            return View(await model.ToListAsync());
        }
        [HttpGet]
        public IActionResult Visa()
        {
            var visa = _context.Visas.Where(x => x.UserId == HttpContext.Session.GetInt32("CustomerId"));
            return View(visa);
        }
        [HttpPost]
        public async Task<IActionResult> AddVisa(decimal ipan)
        {
            var findIpan = await _context.Visas.FirstAsync(x=> x.Ipan == ipan);
            if (findIpan != null)
            {
                findIpan.UserId = HttpContext.Session.GetInt32("CustomerId");
                _context.Visas.Update(findIpan);
                _context.SaveChanges();
                return RedirectToAction("Visa");
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Buy(int total,int quantity , string selectIpan)
        {
            var _id = await _context.Logins.FindAsync(Convert.ToDecimal(HttpContext.Session.GetInt32("LoginId")));
            //var visaID = await _context.Visas.FindAsync(Convert.ToDecimal(selectIpan));
            var visaID = _context.Visas.Where(i => i.Id == int.Parse(selectIpan)).SingleOrDefault();


            if (visaID.Balance < total)
            {
                HttpContext.Session.SetString("message", "Your balance is not enough");
               // ViewBag.Message = "Your balance is not enough ";

                return RedirectToAction("Cart");
            }
            else
            {

                //var itemID = _context.Items.Where(x => x.UserId == HttpContext.Session.GetInt32("CustomerId")).ToList();
                var cart = _context.Carts.Where(u => u.UserId == HttpContext.Session.GetInt32("CustomerId")).Where(s=>s.State=="WAITING").Include(i => i.Item).Include(s=>s.User).ToList();
                Random rnd = new Random();
                int numX = rnd.Next();
                for (int i = 0; i < cart.Count; i++)
                {
                    int num = new Random().Next(); // Move this line inside the loop
                    var sale = new Sale(); // Create a new Sale object for each iteration

                    sale.Id = num;
                    sale.Quantity = cart[i].Quantity;
                    sale.Userid = HttpContext.Session.GetInt32("CustomerId");
                    sale.Itemid = cart[i].ItemId;
                    sale.Dateofsale = DateTime.Now.Date;
                    sale.Totalprice = cart[i].Totalprice;

                    _context.Add(sale);
                    await _context.SaveChangesAsync(); // Use "await" to ensure async operation completes before moving on

                    cart[i].State = "Finished";
                    _context.Update(cart[i]);
                    await _context.SaveChangesAsync(); // Use "await" here as well
                }
                visaID.Balance = visaID.Balance - total;
                _context.Update(visaID);
                _context.SaveChangesAsync();
                Aspose.Pdf.Document document = new Aspose.Pdf.Document();
                Page page = document.Pages.Add();
                Aspose.Pdf.Table table = new Aspose.Pdf.Table();
                table.Border = new Aspose.Pdf.BorderInfo(Aspose.Pdf.BorderSide.All, .5f, Aspose.Pdf.Color.FromRgb(System.Drawing.Color.Aqua));
                table.DefaultCellBorder = new Aspose.Pdf.BorderInfo(Aspose.Pdf.BorderSide.All, .5f, Aspose.Pdf.Color.FromRgb(System.Drawing.Color.Green));
                page.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment("Ran Store"));
                page.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment(" "));
                page.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment("------------------------------------------------------------------------------------------------------------------------"));
                page.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment(" "));
                page.ArtBox.Center();
                {
                    Aspose.Pdf.Row row = table.Rows.Add();
                    row.Cells.Add("Name");
                    row.Cells.Add("Quantity");
                    row.Cells.Add("Price");
                    row.Cells.Add("Description");

                }
                foreach (var item in cart)
                {
                    Aspose.Pdf.Row row = table.Rows.Add();

                    row.Cells.Add(item.Item.Name);
                    row.Cells.Add(item.Quantity.ToString());
                    row.Cells.Add(item.Totalprice.ToString());
                    row.Cells.Add(item.Item.Description);
                }
                page.Paragraphs.Add(table);
                page.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment("------------------------------------------------------------------------------------------------------------------------"));
                page.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment("Total Price : " + total));
                page.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment(" "));
                page.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment("Thank you for shopping, Ran-Store "));
                document.Save(numX + "document.pdf");
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587);
                mail.From = new MailAddress("storeran748@gmail.com");
                mail.To.Add(_id.Username);
                mail.Subject = "Ran-Store";
                mail.Body = "Purchase Invoice";
                smtp.Credentials = new NetworkCredential("storeran748@gmail.com", "ouwsljvfgxqyefpp");
                Attachment data = new Attachment(numX + "document.pdf");
                smtp.EnableSsl = true;
                mail.Attachments.Add(data);
                smtp.Send(mail);

                var cart2 = _context.Carts.Where(u => u.UserId == HttpContext.Session.GetInt32("CustomerId")).Where(s=>s.State=="WAITING").ToList();
               


                return RedirectToAction(nameof(Index));
            }
            
            
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(decimal itemId, int quantity, Cart cart)
        {
            var items = await _context.Items.FindAsync(itemId);
            if (items.UserId == HttpContext.Session.GetInt32("CustomerId"))
            {
                return RedirectToAction("MyItem");
            }
            cart.ItemId = items.Id;
            cart.Quantity = quantity;
            cart.State = "WAITING";
            cart.Totalprice = items.Price * quantity;
            cart.UserId = HttpContext.Session.GetInt32("CustomerId");
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Cart()
        {
            var cart = _context.Carts.Where(x => x.UserId == HttpContext.Session.GetInt32("CustomerId")).Where(S=>S.State=="WAITING").ToList();
            
            var item = _context.Items.ToList();
            var user = _context.Users.ToList();
            var model = from c in cart
                        join i in item
                        on c.ItemId equals i.Id
                        join u in user
                        on c.UserId equals u.Id
                        select new GetCart { Cart = c , Item = i , User = u };
            var getCart = Tuple.Create<IEnumerable<GetCart>>(model);
            // ViewData["totalPrice"] = _context.Carts.Sum(x=>x.TotalPrice);
            ViewData["totalPrice"] = cart.Sum(x => x.Totalprice);
            ViewData["quantity"] = cart.Sum(x => x.Quantity);
            using (ModelContext dbContext = new ModelContext())
            {
                List<Visa> ipans = dbContext.Visas.Where(x => x.UserId == HttpContext.Session.GetInt32("CustomerId")).ToList();

                ViewBag.ipan = new SelectList(ipans, "Id", "Ipan");
            }
            ViewBag.Message = HttpContext.Session.GetString("message");

            return View(getCart);
        }
        public IActionResult Profile()
        {
            var profile = _context.Users.Where(x => x.Id == HttpContext.Session.GetInt32("CustomerId"));
            return View(profile);
        }
        [HttpGet]
        public IActionResult AddItem()
        {
            var category = _context.Categories.ToList();
            return View(category);
        }
        [HttpGet]
        public IActionResult MyItem()
        {
            var item = _context.Items.Where(x => x.UserId == HttpContext.Session.GetInt32("CustomerId")).ToList();
            return View(item);
        }
        [HttpGet]
        public IActionResult AddOffer()
        {
            using (ModelContext dbContext = new ModelContext())
            {
                List<Item> itemsList = dbContext.Items.Where(x => x.UserId == HttpContext.Session.GetInt32("CustomerId")).ToList();

                ViewBag.Items = new SelectList(itemsList, "Id", "Name");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOffer([Bind("Id,DiscountPer,Price,Status")] Offer offer,int newPrice, string item)
        { 
            if (ModelState.IsValid)
            {
                offer.Status = "NotAccept";
                var _id = Convert.ToDecimal(item);
                var items = await _context.Items.FindAsync(_id);
                offer.ItemId = items.Id;
                offer.DiscountPer = (newPrice/items.Price) *100;
                offer.Price = newPrice;
                _context.Offers.Add(offer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(offer);
        }
            [HttpGet]
        public IActionResult Item()
        {
            using (ModelContext dbContext = new ModelContext())
            {
                List<Category> categoriesList = dbContext.Categories.ToList();

                ViewBag.Categories = new SelectList(categoriesList, "Id", "Name");
            }
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Item( [Bind("Id,Name,Description,Status,Price,Dateofupload,ImageFile")] Item item , string category)
        {
            if (ModelState.IsValid)
            {
                if (item.ImageFile != null)
                {
                    var wwwRootPath = _environment.WebRootPath;
                    var fileName = Guid.NewGuid().ToString() + "_" + item.ImageFile.FileName;
                    var path = Path.Combine(wwwRootPath + "/Images/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await item.ImageFile.CopyToAsync(fileStream);
                    }
                    item.Imagepath = fileName;
                }
                item.Status = "NotAccept";
                
                item.UserId = HttpContext.Session.GetInt32("CustomerId");
                var _id = Convert.ToDecimal(category);
                var cat = await _context.Categories.FindAsync(_id);
                item.CategoryId = cat.Id;
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        public IActionResult AddTestimonial()
        {
            var testimonial = _context.Testimonials.Where(t => t.Status == "Accept").ToList();
            var user = _context.Users.ToList();
            var model = from u in user
                        join t in testimonial
                        on u.Id equals t.UserId
                        select new ModelTestimoinal { Testimoinal = t, User = u };
            var tuple = Tuple.Create<IEnumerable<ModelTestimoinal>>(model);
            return View(tuple);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTestimonial(string test, Testimonial testimonial)
        { 
            if(ModelState.IsValid)
            {
                testimonial.Status = "NotAccept";
                testimonial.Message = test;
                testimonial.UserId= HttpContext.Session.GetInt32("CustomerId");
                _context.Add(testimonial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        }
}
