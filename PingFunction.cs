using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureMIAuth
{
    public class PingFunction
    {
        private readonly ILogger<PingFunction> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public PingFunction(ILogger<PingFunction> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [Function("PingFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");


            try
            {
                var client = _httpClientFactory.CreateClient("apim");

                var response = await client.GetAsync("/test-api/ping");

                return new OkObjectResult($"{response.StatusCode}: {response.ReasonPhrase}");


            }
            catch (Exception e)
            {
                return new OkObjectResult($"{e.Message}");
            }
        }
    }
}
