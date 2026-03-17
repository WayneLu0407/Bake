using Bake.Areas.Seller.ViewModels;
using Bake.Data;
using Bake.Models.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace Bake.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Roles = "Seller")]
    public class SellerProductController : Controller
    {

        private readonly BakeContext _context;
        private readonly IWebHostEnvironment _env;

        public SellerProductController(BakeContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        private int GetCurrentUserId() //取得目前登入的 UserId
        {
            return int.Parse(User.FindFirstValue("UserId"));
        }



        // GET: Products/Index
        [HttpGet]
        public async Task<IActionResult> All()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return Content($"錯誤：{ex.Message}\n\n{ex.StackTrace}");
            }
        }

        // Post: Products/IndexJson
        [HttpPost]
        public async Task<JsonResult> AllJson()
        {
            var userId = GetCurrentUserId();
            var products = await _context.Products
                .Where(p => p.UserId == userId)
                .Include(p => p.ProductDetail)
                .Include(p => p.Category)
                .Include(p => p.ProductIngredient)
                .Select(p => new
                {
                    ProductDate = p.ProductDate,
                    productId = p.ProductId,
                    productName = p.ProductName,
                    categoryName = p.Category != null ? p.Category.CategoryName : "未分類",
                    productDescription = p.ProductDescription,
                    productImage = p.ProductImage,
                    productPrice = p.ProductDetail != null ? p.ProductDetail.ProductPrice : 0,
                    productDiscount = p.ProductDetail != null ? p.ProductDetail.ProductDiscount : 0,
                    productQuantity = p.ProductDetail != null ? p.ProductDetail.ProductQuantity : 0,
                    productExpireDate = p.ProductDetail != null && p.ProductDetail.ExpireDate != null
                    ? p.ProductDetail.ExpireDate: null,
                    ingredients = p.ProductIngredient != null ? p.ProductIngredient.Ingredients : "",
                    netWeight = p.ProductIngredient != null ? p.ProductIngredient.NetWeight : "",
                    shelfLifeNote = p.ProductIngredient != null ? p.ProductIngredient.ShelfLifeNote : ""
                }).ToListAsync();

            // 在 C# 端格式化日期
            var result = products.Select(p => new
            {
                p.productId,
                p.productName,
                p.categoryName,
                p.productDescription,
                p.productImage,
                p.productPrice,
                p.productDiscount,
                p.productQuantity,
                p.ingredients,
                p.netWeight,
                p.shelfLifeNote,
                productExpireDate = p.productExpireDate != null ? p.productExpireDate.Value.ToString("yyyy/MM/dd") : "",
                ProductDate = p.ProductDate.ToString("yyyy/MM/dd") 
            });

            return Json(result);
        }
        [HttpGet]
        public async Task<JsonResult> GetProduct(int id)
        {
            var userId = GetCurrentUserId();
            var product = await _context.Products
            .Include(p => p.ProductDetail)
            .Where(p => (p.ProductId == id && p.UserId == userId))
            .Select(p => new
            {
                productId = p.ProductId,
                productName = p.ProductName,
                productDate = p.ProductDate,
                categoryId = p.CategoryId,
                productImage = p.ProductImage,
                productDescription = p.ProductDescription,
                productPrice = p.ProductDetail != null ? p.ProductDetail.ProductPrice : 0,
                productDiscount = p.ProductDetail != null ? p.ProductDetail.ProductDiscount : 0,
                productQuantity = p.ProductDetail != null ? p.ProductDetail.ProductQuantity : 0,
                productExpireDate = p.ProductDetail != null ? p.ProductDetail.ExpireDate : null,
                ingredients = p.ProductIngredient != null ? p.ProductIngredient.Ingredients : null,
                netWeight = p.ProductIngredient != null ? p.ProductIngredient.NetWeight : null,
                shelfLifeNote = p.ProductIngredient != null ? p.ProductIngredient.ShelfLifeNote : null,
            })
            .FirstOrDefaultAsync();

           return Json(product);
        }

        // Modal 儲存用
        [HttpPost]
        public async Task<JsonResult> EditJson(int ProductId, string ProductName, string? ProductDescription, decimal ProductPrice, int ProductQuantity, decimal ProductDiscount, int CategoryId, DateTime? ExpireDate, IFormFile? ProductImage, string? Ingredients, string? NetWeight, string? ShelfLifeNote)
        {
            try
            {
                var userId = GetCurrentUserId();
                var product = await _context.Products
                    .Where(p => p.UserId == userId)
                    .Include(p => p.ProductDetail)
                    .Include(p => p.ProductIngredient)
                    .FirstOrDefaultAsync(p => p.ProductId == ProductId);
                if (product == null)
                    return Json(new { success = false, message = "找不到商品" });

                product.ProductName = ProductName;
                product.ProductDescription = ProductDescription;
                product.CategoryId = CategoryId;

                if (ProductImage != null && ProductImage.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProductImage.FileName);
                    string savePath = Path.Combine(_env.WebRootPath, "ProductPicture", fileName);
                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await ProductImage.CopyToAsync(stream);
                    }
                    product.ProductImage = "/ProductPicture/" + fileName;
                }
                if (product.ProductDetail != null)
                {
                    product.ProductDetail.ProductPrice = ProductPrice;
                    product.ProductDetail.ProductDiscount = ProductDiscount;
                    product.ProductDetail.ProductQuantity = ProductQuantity;
                    product.ProductDetail.ExpireDate = ExpireDate;
                }

                if (product.ProductIngredient != null)
                {
                    product.ProductIngredient.Ingredients = Ingredients;
                    product.ProductIngredient.NetWeight = NetWeight;
                    product.ProductIngredient.ShelfLifeNote = ShelfLifeNote;
                }
                else 
                {
                    product.ProductIngredient = new ProductIngredient
                    {
                        Ingredients = Ingredients,
                        NetWeight = NetWeight,
                        ShelfLifeNote = ShelfLifeNote
                    };
                }
                if (ExpireDate.HasValue && ExpireDate.Value < product.ProductDate.Date)
                {
                    return Json(new { success = false, message = "過期日不能早於上架日" });
                }
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        public async Task<JsonResult> GetCategories()
        {
            var categories = await _context.ProductCategories
                .Select(c => new
                {
                    categoryId = c.CategoryId,
                    categoryName = c.CategoryName
                })
                .ToListAsync();

            return Json(categories);
        }

        // AJAX 刪除用
        [HttpPost]
        public async Task<JsonResult> DeleteJson(int id)
        {
            var userId = GetCurrentUserId();
            var product = await _context.Products
                .Where(p => p.UserId == userId)
                .Include(p => p.ProductDetail)  // ← 一起載入
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product != null)
            {
                if (product.ProductDetail != null)
                {
                    _context.Remove(product.ProductDetail);  // ← 先刪 Detail
                }
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "找不到商品" });
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
        [RequestFormLimits(MultipartBodyLengthLimit = 5242880)]
        [RequestSizeLimit(5242880)]
        public async Task<IActionResult> New([Bind("ProductName,ProductDescription,CategoryId,ProductImage,ProductPrice,ProductDiscount,ProductQuantity,ShelfLifeNote,Ingredients,NetWeight,ExpireDate")] ProductViewModel item)
        {
            if (item.ExpireDate < DateTime.Now.Date)
            {
                ModelState.AddModelError("ExpireDate", "過期日不能早於今天");
            }
            if (ModelState.IsValid)
            {

                var product = new Product
                {
                    ProductName = item.ProductName!,
                    ProductDescription = item.ProductDescription,
                    UserId = GetCurrentUserId(),
                    ProductDate = DateTime.Now,
                    CategoryId = item.CategoryId,
                    ProductMethod = "未設定"
                };

                var detail = new ProductDetail
                {
                    ProductPrice = item.ProductPrice,
                    ProductDiscount = item.ProductDiscount,
                    ProductQuantity = item.ProductQuantity,
                    ExpireDate = item.ExpireDate,
                };
                product.ProductDetail = detail;

                var ingredient = new ProductIngredient
                {
                    ShelfLifeNote = item.ShelfLifeNote,
                    Ingredients = item.Ingredients,
                    NetWeight = item.NetWeight
                };
                product.ProductIngredient = ingredient;

                if (item.ProductImage != null && item.ProductImage.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(item.ProductImage.FileName);
                    string savePath = Path.Combine(_env.WebRootPath, "ProductPicture", fileName);
                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await item.ProductImage.CopyToAsync(stream);
                    }
                    product.ProductImage = "/ProductPicture/" + fileName;
                }
                else
                {
                    product.ProductImage = "/ProductPicture/NoImage.jpg";
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
        [RequestFormLimits(MultipartBodyLengthLimit = 5242880)]
        [RequestSizeLimit(5242880)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductDetail)
                .Where(p => p.ProductId == id)
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription != null ? p.ProductDescription : null,
                    ProductImagePath = p.ProductImage,
                    ProductPrice = p.ProductDetail != null ? p.ProductDetail.ProductPrice : 0,
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
        public async Task<IActionResult> Edit(int? id, [Bind("ProductId,ProductName,ProductDescription,CategoryId,ProductImage,ProductPrice,ProductDiscount,ProductQuantity")] ProductViewModel item)
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

    }
}     