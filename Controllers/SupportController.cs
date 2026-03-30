using Microsoft.AspNetCore.Mvc;
using Azure.Identity;
using Azure.AI.OpenAI;
using OpenAI.Chat;
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

        [HttpPost]
        public async Task<IActionResult> GetAiResponse(string userQuestion)
        {
            if (string.IsNullOrEmpty(userQuestion))
                return BadRequest("請輸入問題");

            try
            {
                // appsettings.json 裡改放 Azure OpenAI 的 endpoint
                string endpoint = _config["AzureAI:Endpoint"];     // e.g. https://xxx.openai.azure.com/
                string apiKey = _config["AzureAI:ApiKey"];        // 或改用 DefaultAzureCredential

                var azureClient = new AzureOpenAIClient(
                    new Uri(endpoint),
                    new System.ClientModel.ApiKeyCredential(apiKey)
                // 若用 Managed Identity 改成：new DefaultAzureCredential()
                );

                ChatClient chatClient = azureClient.GetChatClient(deploymentName);

                var messages = new List<ChatMessage>
                {
                    new SystemChatMessage("你是一個 Bake 甜點電商平台的專屬 AI 客服，請用親切、專業的語氣回答問題。"),
                    new UserChatMessage(userQuestion)
                };

                ChatCompletion completion = await chatClient.CompleteChatAsync(messages);
                string answer = completion.Content[0].Text;

                return Json(new { answer });
            }
            catch (Exception ex)
            {
                return Json(new { answer = "連線失敗：" + ex.Message });
            }
        }
    }
}
