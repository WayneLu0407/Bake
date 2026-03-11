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
                    CategoryId = item.CategoryId!.Value,
                    ProductMethod = "未設定"
                };

                var detail = new ProductDetail
                {
                    ProductId = product.ProductId,
                    ProductPrice = item.ProductPrice ?? 1,
                    ProductDiscount = item.ProductDiscount ??0,
                    ProductQuantity = item.ProductQuantity ?? 0,
                };


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
                return RedirectToAction("All", "SellerProduct");

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
            var product = _context.Products
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
                }).FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        // POST: SellerProduct/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductDescription,ProductImage")] Product product)
        {

            if (ModelState.IsValid)
            {
                Product c = await _context.Products.FindAsync(product.ProductId);
                if (Request.Form.Files["Picture"] != null)
                {
                    using (BinaryReader br = new BinaryReader(Request.Form.Files["ProductImage"].OpenReadStream()))
                    {
                        product.ProductImage = br.ReadBytes((int)Request.Form.Files(Path.Combine(_env.WebRootPath, "ProductPicture", fileName)).Length);
                    }
                }
                else
                {
                    product.ProductImage = c.ProductImage;
                }
                _context.Entry(c).State = EntityState.Detached;
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }




        public IActionResult All()
        {
            return View();
        }
    }
}
