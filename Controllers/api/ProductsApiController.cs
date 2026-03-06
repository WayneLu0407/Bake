using Bake.Data;
using Microsoft.AspNetCore.Mvc;

namespace Bake.Controllers.api
{
    [Route("api/Product/[action]")]
    [ApiController]
    public class ProductApiController : ControllerBase
    {
        private readonly BakeContext _db;

        public ProductApiController(BakeContext db)
        {
            this._db = db;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var prod = _db.Products.ToList();
            return Ok(prod);
        }
    }
}
