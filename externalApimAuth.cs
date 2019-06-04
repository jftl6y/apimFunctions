using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Services.AppAuthentication;

namespace Microsoft.FastTrack
{
    public static class externalApimAuth
    {
        [FunctionName("externalApimAuth")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            log.LogInformation("C# HTTP trigger function processed a request.");
            //Get apiKey from request body or params
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string apiKey = req.Query["apiKey"];
            apiKey = apiKey ?? data?.apiKey;

            log.LogInformation($"apiKey value is {apiKey}");

            //Get Environment Variables
            var tenantId = Environment.GetEnvironmentVariable("tenantId");
            var subscriptionId = Environment.GetEnvironmentVariable("subscriptionId");
            var apimResourceGroup = Environment.GetEnvironmentVariable("apimResourceGroup");
            var apimInstance = Environment.GetEnvironmentVariable("apimInstanceName");
                       
            //Get auth token
            log.LogInformation("Getting bearer token");
            string bearerToken = await GetAccessToken(tenantId, log);
            log.LogInformation($"Received bearer token {bearerToken.Substring(0, 10)}....");

            //Build REST client
            HttpClient client = new HttpClient();
            string url = $"https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/{apimResourceGroup}/providers/Microsoft.ApiManagement/service/{apimInstance}/subscriptions";
            string urlParameters = "?api-version=2019-01-01";
            client.BaseAddress = new Uri(url);            
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
            try {
                log.LogInformation("Getting subscription keys");
                var result = client.GetStringAsync(urlParameters).Result;
                //Deserialize into the return object
                var resultJson = JsonConvert.DeserializeObject<ApimSubscriptions>(result);
                log.LogInformation("Retrieved subscription keys");
                
                List<Value> resultValues = new List<Value>(resultJson.value);
                bool apiKeyFound = resultValues.Where(m => m.properties.primaryKey == apiKey).FirstOrDefault() is null ?
                    resultValues.Where(m => m.properties.secondaryKey == apiKey).FirstOrDefault() is null ? false : true 
                    : true;
                log.LogInformation($"apiKeyFound: {apiKeyFound.ToString()}");
                //If the API Key was found in the subscription list, pass back a 202 (Accepted) , else 401 (Unauthorized)
                return apiKeyFound ? (ActionResult)new AcceptedResult()
                : new UnauthorizedResult();
            }
            catch (Exception ex)
            {
                log.LogError("Exception caught in function execution",ex);
                log.LogError(ex.Message + ex.StackTrace);
                return new UnauthorizedResult();
            }
        }
        private static async Task<string> GetAccessToken(string tenantId, ILogger log)
        {
            try
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var accessToken = await azureServiceTokenProvider.GetAccessTokenAsync("https://management.azure.com",tenantId);
                return accessToken;
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message, ex);
                throw ex;
            }

        }
    }
}
