using Bake.Data;
using Bake.Models;
using Bake.Models.Sales;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace Bake.Areas.Seller.Controllers
{
    [Area("Seller")]
    public class SellerController : Controller
    {
        private readonly BakeContext _context;
        private readonly IWebHostEnvironment _env;

        public SellerController(BakeContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult New() => View();

        // POST: Seller/New
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New([Bind("ProductName,ProductDescription,CategoryId")] Product item)
        {
            if (ModelState.IsValid) 
            {
                item.UserId = 1;
                item.ProductDate = DateTime.Now;
                var file = Request.Form.Files["ProductImage"];
                if (file!= null && file.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string savePath = Path.Combine(_env.WebRootPath, "ProductPicture", fileName);
                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    item.ProductImage = "ProductPicture/" + fileName;
                }
                else
                {
                    item.ProductImage = "ProductPicture/NoImage.jpg";
                }
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Dashboard));

            }
            return View(item);
        }

        
        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult Orders()
        {
            return View();
        }

        public IActionResult Past()
        {
            return View();
        }

        public IActionResult Shop_settings()
        {
            return View();
        }

        public IActionResult Billing_account()
        {
            return View();
        }

        public IActionResult Shop()
        {
            return View();
        }
    }
}
