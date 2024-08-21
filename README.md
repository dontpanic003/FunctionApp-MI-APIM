# Intro
This code sample demonstrates how to make a call from Azure Function App to Azure APIM using System assigned Azure Managed Identity.

# Prerequesties
- Azure tenancy with tenant id AZURE_TENANT_ID
- Azure Function app with system assigned managed identity with app id AZURE_MI_APP_ID
- APIM instance with gateway URL AZURE_APIM_GATEWAY_URL.
- GET /test-api/ping endpoint on APIM instance
- endpoint policy includes:
  ```
  <validate-azure-ad-token tenant-id="AZURE_TENANT_ID">
    <client-application-ids>
      <application-id>AZURE_MI_APP_ID</application-id>
    </client-application-ids>
  </validate-azure-ad-token>
  ```
- Function app environmental variables:
  1. ApimConfiguration:BaseAddress : AZURE_APIM_GATEWAY_URL

# How it works
## Add DefaultAzureCredential to http client in Program.cs (here named client used as an example)
```
services.AddHttpClient("apim",client =>
{
    var apimBaseAddress = config.GetSection("ApimConfiguration:BaseAddress").Value;

    client.BaseAddress = new Uri(apimBaseAddress);
    var cred = new DefaultAzureCredential();
    var token = cred.GetToken(new TokenRequestContext(new[] { "https://management.azure.com/.default" }));
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
});
```
## Inject IHttpClientFactory
```
private readonly IHttpClientFactory _httpClientFactory;

public PingFunction(..., IHttpClientFactory httpClientFactory)
{
    ...
    _httpClientFactory = httpClientFactory;
}
```

## Use client in a GET request
```
var client = _httpClientFactory.CreateClient("apim");

var response = await client.GetAsync("/test-api/ping");
```

