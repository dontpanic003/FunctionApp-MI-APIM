using System.Net.Http.Headers;
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        var environment = hostingContext.HostingEnvironment.EnvironmentName;

        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
        config.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false);
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: false);
        config.AddEnvironmentVariables();
        config.AddUserSecrets<Program>(optional: true, reloadOnChange: true);
    })
    .ConfigureServices((hostContext,services) =>
    {
        var config = hostContext.Configuration;

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddHttpClient("apim",client =>
        {
            var apimBaseAddress = config.GetSection("SqlConfiguration:ConnectionString").Value;

            client.BaseAddress = new Uri(apimBaseAddress);
            var cred = new DefaultAzureCredential();
            var token = cred.GetToken(new TokenRequestContext(new[] { "https://management.azure.com/.default" }));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        });
    })
    .Build();

host.Run();
