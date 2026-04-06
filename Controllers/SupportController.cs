using Microsoft.AspNetCore.Mvc;
using Azure;
using Azure.AI.Language.QuestionAnswering;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Bake.Controllers
{
    public class SupportController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string deploymentName = "gpt-4o-mini";
        
        // 透過建構子注入 IConfiguration
        public SupportController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Faq()
        {
            return View();
        }
        public IActionResult Order_lookup()
        {
            return View();
        }
        public IActionResult AiRobot()
        {
            return View();
        }

        
    }
}
