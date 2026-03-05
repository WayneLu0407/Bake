using Bake.Data;
using Bake.Models;
using Bake.Models.Sales;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Bake.Areas.Seller.ViewModels;



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
        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }
        [HttpGet]
        public IActionResult New() => View();

        // POST: Seller/New
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New([Bind("ProductName,ProductDescription,CategoryId")] ProductViewModel item)
        {
            if (ModelState.IsValid) 
            {
                var product = new Product
                {
                    ProductName = item.ProductName,
                    ProductDescription = item.ProductDescription,
                    UserId = 1, // 暫時固定
                    ProductDate = DateTime.Now,
                    CategoryId = 1 // 暫時固定
                };


                var file = Request.Form.Files["ProductImage"];
                if (file!= null && file.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string savePath = Path.Combine(_env.WebRootPath, "ProductPicture", fileName);
                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    product.ProductImage = "ProductPicture/" + fileName;
                }
                else
                {
                    product.ProductImage = "ProductPicture/NoImage.jpg";
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
