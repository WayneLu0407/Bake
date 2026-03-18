using Bake.Data;
using Bake.ViewModel.ShopFront;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bake.Controllers
{
    public class ShopController : Controller
    {
        private readonly BakeContext _context;

        public ShopController(BakeContext context)
        {
            _context = context;
        }

        [HttpGet("/Shop/{id:int}")]
        public async Task<IActionResult> Index(
            int id,
            string? keyword,
            int? categoryId,
            string? sort = "latest",
            string? priceRange = "all",
            int page = 1)
        {
            var vm = await BuildShopProfilePageViewModel(id, keyword, categoryId, sort, priceRange, page);

            if (vm == null)
            {
                return NotFound();
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ShopProfileProductListPartial", vm);
            }

            return View(vm);
        }

        private async Task<ShopProfilePageViewModel?> BuildShopProfilePageViewModel(
            int id,
            string? keyword,
            int? categoryId,
            string? sort,
            string? priceRange,
            int page)
        {
            const int pageSize = 12;

            if (page < 1)
            {
                page = 1;
            }

            sort = string.IsNullOrWhiteSpace(sort) ? "latest" : sort.Trim().ToLower();
            priceRange = string.IsNullOrWhiteSpace(priceRange) ? "all" : priceRange.Trim().ToLower();

            var shop = await _context.Shops
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == id);

            if (shop == null)
            {
                return null;
            }

            var productQuery = _context.Products
                .AsNoTracking()
                .Where(p => p.UserId == id)
                .Include(p => p.Category)
                .Include(p => p.ProductDetail)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                productQuery = productQuery.Where(p =>
                    EF.Functions.Like(p.ProductName, $"%{keyword}%"));
            }

            if (categoryId.HasValue)
            {
                productQuery = productQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            productQuery = priceRange switch
            {
                "0-100" => productQuery.Where(p => 
                    p.ProductDetail != null && 
                    p.ProductDetail.ProductPrice < 100),
                "100-300" => productQuery.Where(p => 
                    p.ProductDetail != null && 
                    p.ProductDetail.ProductPrice >= 100 &&
                    p.ProductDetail.ProductPrice < 300),
                "300-500" => productQuery.Where(p => 
                    p.ProductDetail != null && 
                    p.ProductDetail.ProductPrice >= 300 && 
                    p.ProductDetail.ProductPrice < 500),
                "500-up" => productQuery.Where(p => 
                    p.ProductDetail != null && 
                    p.ProductDetail.ProductPrice >= 500),
                _ => productQuery
            };

            productQuery = sort switch
            {
                "price_asc" => productQuery.OrderBy(p => 
                    p.ProductDetail != null ? 
                    p.ProductDetail.ProductPrice : 0),
                "price_desc" => productQuery.OrderByDescending(p => 
                    p.ProductDetail != null ? p.ProductDetail.ProductPrice : 0),
                "rating" => productQuery.OrderByDescending(p => 
                    p.ProductRating ?? 0),
                _ => productQuery.OrderByDescending(p => 
                    p.ProductDate)
            };

            var categories = await _context.Products
                .AsNoTracking()
                .Where(p => p.UserId == id)
                .GroupBy(p => new
                {
                    p.CategoryId,
                    CategoryName = p.Category.CategoryName
                })
                .Select(g => new ShopCategoryFilterItemViewModel
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    Count = g.Count()
                })
                .OrderBy(x => x.CategoryId)
                .ToListAsync();

            var totalCount = await productQuery.CountAsync();

            var products = await productQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ShopProductCardViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductImage = p.ProductImage,
                    ShopName = shop.ShopName,
                    CategoryName = p.Category.CategoryName,
                    Price = p.ProductDetail != null ? p.ProductDetail.ProductPrice : 0,
                    OriginalPrice =
                        p.ProductDetail != null &&
                        p.ProductDetail.ProductDiscount.HasValue &&
                        p.ProductDetail.ProductDiscount.Value > 0 &&
                        p.ProductDetail.ProductDiscount.Value < 1
                            ? Math.Round(
                                p.ProductDetail.ProductPrice /
                                (1 - p.ProductDetail.ProductDiscount.Value), 0)
                            : null,
                    Rating = p.ProductRating
                })
                .ToListAsync();

            return new ShopProfilePageViewModel
            {
                ShopInfo = shop,
                Products = products,
                Categories = categories,
                Keyword = keyword,
                CategoryId = categoryId,
                Sort = sort,
                PriceRange = priceRange,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}