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

        [HttpGet]
        [RequestFormLimits(MultipartBodyLengthLimit = 2048000)]
        [RequestSizeLimit(2048000)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p=>p.ProductDetail)
                .Where(p => p.ProductId == id)
                .Select(p => new ProductViewModel
                {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductDescription = p.ProductDescription != null ? p.ProductDescription:null,
                ProductImagePath = p.ProductImage,
                ProductPrice = p.ProductDetail != null ? p.ProductDetail.ProductPrice: 0,
                ProductDiscount = p.ProductDetail != null ? p.ProductDetail.ProductDiscount : 0,
                ProductQuantity = p.ProductDetail != null ? p.ProductDetail.ProductQuantity : 0,
                ExpireDate = p.ProductDetail != null && p.ProductDetail.ExpireDate != null
                         ? p.ProductDetail.ExpireDate.Value
                         : DateTime.Now,
                }).FirstOrDefaultAsync(m => m.ProductId == id);


            if (product == null)
            {
                return NotFound();
            }
            ViewBag.Categories = await _context.ProductCategories
            .Select(c => new { c.CategoryId, c.CategoryName })
            .ToListAsync();
            return View(product);
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 2048000)]
        [RequestSizeLimit(2048000)]
        public async Task<IActionResult> Edit(int? id,[Bind("ProductId,ProductName,ProductDescription,CategoryId,ProductImage,ProductPrice,ProductDiscount,ProductQuantity")] ProductViewModel item)
        {
            if (ModelState.IsValid)
            {
                var product = await _context.Products
                .Include(p => p.ProductDetail)
                .FirstOrDefaultAsync(p => p.ProductId == item.ProductId);

                if (product == null) return NotFound();

                product.ProductName = item.ProductName!;
                product.ProductDescription = item.ProductDescription;
                product.CategoryId = item.CategoryId;

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
                if (product.ProductDetail != null)
                {
                    product.ProductDetail.ProductPrice = item.ProductPrice;
                    product.ProductDetail.ProductDiscount = item.ProductDiscount;
                    product.ProductDetail.ProductQuantity = item.ProductQuantity;
                }

                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["Fixed"] = "商品修改成功！";
                    return RedirectToAction("All");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(p => p.ProductId == item.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewBag.Categories = await _context.ProductCategories
            .Select(c => new { c.CategoryId, c.CategoryName })
            .ToListAsync();
            return View(item);
        }

        public async Task<IActionResult> All()
        {
            var products = await _context.Products
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
                .ToListAsync();

            return View(products); // ← 把資料傳給 View
        }
    }
}
