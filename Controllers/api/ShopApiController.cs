using Bake.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bake.Controllers.api
{
    [Route("api/Shop/[action]")]
    [ApiController]
    public class ShopApiController : ControllerBase
    {
        private readonly BakeContext _db;
        public ShopApiController(BakeContext db)
        {
            this._db = db;
        }

        // 取全部商店 → /api/Shop/Get
        [HttpGet]
        public IActionResult Get()
        {
            var shop = _db.Shops
                .Include(s => s.Status)
                .Select(s => new {
                    userId = s.UserId,
                    shopName = s.ShopName,
                    shopImg = s.ShopImg,
                    shopRating = s.ShopRating,
                    shopTime = s.ShopTime,
                    shopDescription = s.ShopDescription,
                    statusId = s.StatusId
                })
                .ToList();
            return Ok(shop);
        }

        // 取單筆商店 → /api/Shop/GetById/3
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var shop = _db.Shops
                .Include(s => s.Status)
                .Where(s => s.UserId == id)
                .Select(s => new {
                    userId = s.UserId,
                    shopName = s.ShopName,
                    shopImg = s.ShopImg,
                    shopRating = s.ShopRating,
                    shopTime = s.ShopTime,
                    shopDescription = s.ShopDescription,
                    statusId = s.StatusId
                })
                .FirstOrDefault();

            if (shop == null) return NotFound();
            return Ok(shop);

        }
    }
}
