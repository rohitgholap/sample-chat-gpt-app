using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
using OpenAI_API.Completions;
using System.Net.Mail;
using Microsoft.Extensions.Logging;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatGPT.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatGPTController : ControllerBase
    {

        private readonly IConfiguration Configuration;
        private readonly ILogger<ChatGPTController> Logger;

        public ChatGPTController(IConfiguration configuration, ILogger<ChatGPTController> logger)
        {
            this.Configuration = configuration;
            this.Logger = logger;
        }

        [HttpGet(Name = "ChatGPT")]
        public async Task<IActionResult> GetOpenAIResponse(string query)
        {
            try
            {
                var secretKey = Configuration.GetValue<string>("OpenAI-SecretKey");
                if (string.IsNullOrWhiteSpace(secretKey))
                {
                    Logger.LogCritical("no secret key found for the chatGPT application, please check the config file");
                    return StatusCode(500);
                }

                string chatGPTResult = string.Empty;
                var openAI = new OpenAIAPI(secretKey);
                CompletionRequest request = new CompletionRequest();
                request.Prompt = query;
                request.Model = OpenAI_API.Models.Model.DavinciText;

                var results = await openAI.Completions.CreateCompletionAsync(request);

                foreach(var result in results.Completions)
                {
                    chatGPTResult = result.Text;
                }
                Logger.LogInformation("chat-gpt request completed successfully!!");
                return Ok(chatGPTResult);

            }
            catch(Exception ex)
            {
                var errorMessage = "exception thrown in chat gpt application";
                Logger.LogError($" {errorMessage} {ex.Message}");                

                return StatusCode(StatusCodes.Status500InternalServerError, new { errorMessage = errorMessage });               

            }
        }
    }
}

