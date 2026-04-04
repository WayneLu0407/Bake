using Azure;
using Azure.AI.Language.QuestionAnswering;
using Microsoft.AspNetCore.Mvc;

namespace Bake.Controllers.api
{
    [ApiController]
    [Route("api/SupportApi")]
    public class SupportApiController : ControllerBase
    {
        private readonly IConfiguration _config;

        public SupportApiController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChatRequest request)
        {
            // 從 appsettings.json 讀取（安全）
            var key = _config["AzureQA:ApiKey"];
            var endpoint = _config["AzureQA:Endpoint"];

            var client = new QuestionAnsweringClient(
                new Uri(endpoint),
                new AzureKeyCredential(key)
            );

            var project = new QuestionAnsweringProject(
                "bakeQnA",
                "production"
            );

            AnswersResult result = await client.GetAnswersAsync(
                request.Question, project
            );

            var bestAnswer = result.Answers[0];

            if (bestAnswer.Confidence < 0.5)
            {
                return Ok(new
                {
                    Answer = "感謝您的提問！很抱歉，我目前無法回答這個問題。您可以嘗試換個方式描述您的問題，或者透過以下方式聯繫我們的客服團隊，將有專人為您服務： 客服信箱：support@sweetstack.com 我們會盡快回覆您！",
                    ConfidenceScore = bestAnswer.Confidence
                });
            }

            return Ok(new
            {
                Answer = bestAnswer.Answer,
                ConfidenceScore = bestAnswer.Confidence
            });
        }
    }

    public class ChatRequest
    {
        public string Question { get; set; }
    }
}