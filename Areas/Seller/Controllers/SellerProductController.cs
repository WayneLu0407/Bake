using Bake.Areas.Seller.ViewModels;
using Bake.Data;
using Bake.Models.Sales;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Bake.Areas.Seller.Controllers
{
    [Area("Seller")]
    public class SellerProductController : Controller
    {

        private readonly BakeContext _context;
        private readonly IWebHostEnvironment _env;

        public SellerProductController(BakeContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> New()
        {
            ViewBag.Categories = await _context.ProductCategories
                .Select(c => new { c.CategoryId, c.CategoryName })
                .ToListAsync();
            return View();
        }

        // POST: Seller/New
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New([Bind("ProductName,ProductDescription,CategoryId,ProductImage,ProductPrice,ProductDiscount,ProductQuantity")] ProductViewModel item)
        {
            if (ModelState.IsValid)
            {

                var product = new Product
                {
                    ProductName = item.ProductName!, 
                    ProductDescription = item.ProductDescription,
                    UserId = 1, // 暫時固定
                    ProductDate = DateTime.Now,
                    CategoryId = item.CategoryId,
                    ProductMethod = "未設定"
                };

                var detail = new ProductDetail
                {
                    ProductPrice = item.ProductPrice,
                    ProductDiscount = item.ProductDiscount,
                    ProductQuantity = item.ProductQuantity,
                    ExpireDate= DateTime.Now,
                };
                product.ProductDetail = detail;


                if (item.ProductImage != null && item.ProductImage.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(item.ProductImage.FileName);
                    string savePath = Path.Combine(_env.WebRootPath, "ProductPicture", fileName);
                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await item.ProductImage.CopyToAsync(stream);
                    }
                    product.ProductImage = "ProductPicture/" + fileName;
                }
                else
                {
                    product.ProductImage = "ProductPicture/NoImage.jpg";
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                ViewBag.Categories = await _context.ProductCategories
                .Select(c => new { c.CategoryId, c.CategoryName })
                .ToListAsync();
                TempData["Success"] = "商品新增成功！";
                return RedirectToAction("All");

            }
            ViewBag.Categories = await _context.ProductCategories
            .Select(c => new { c.CategoryId, c.CategoryName })
            .ToListAsync();
            return View(item);
        }




        [RequestFormLimits(MultipartBodyLengthLimit = 2048000)]
        [RequestSizeLimit(2048000)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var productData = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductDetail)
                .Select(p => new ProductListViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductImage = p.ProductImage,
                    ProductDescription = p.ProductDescription,
                    CategoryName = p.Category != null ? p.Category.CategoryName : "未分類",
                    ProductQuantity = p.ProductDetail != null ? p.ProductDetail.ProductQuantity : 0
                })
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (productData == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await _context.ProductCategories
                    .Select(c => new { c.CategoryId, c.CategoryName })
                    .ToListAsync();

            return View(productData);
        }
        public IActionResult All()
        {
            return View();
        }
    }
}
