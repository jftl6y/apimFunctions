using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using Newtonsoft.Json;


namespace Microsoft.FastTrack
{
    public static class convertXmlToJson
    {
        [FunctionName("convertXmlToJson")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            const string soapEnvelopeToken = "<soapenv:Envelope";
            const string xmlnsToken = "xmlns:json='http://james.newtonking.com/projects/json'";
            const string itemToken = "<yfc:Item ";
            const string jsonArrayToken = " json:Array='true' ";
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            requestBody = requestBody.Replace(soapEnvelopeToken, $"{soapEnvelopeToken} {xmlnsToken}");
                requestBody = requestBody.Replace(itemToken, $"{itemToken} {jsonArrayToken}");
                
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(requestBody);
                
                string convertFromXml = JsonConvert.SerializeXmlNode(doc);
            return (ActionResult)new OkObjectResult(convertFromXml);
        }
    }
}