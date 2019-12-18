using System;
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
        /// <summary>
        /// Demonstrates converting between XML and JSON while specifying an XML array attribute to force a JSON array for single lines in repeating XML elements.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("convertXmlToJson")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            try { 
                /*
                const string soapEnvelopeToken = "<soapenv:Envelope";
                const string xmlnsToken = "xmlns:json='http://james.newtonking.com/projects/json'";
                const string itemToken = "<yfc:Item ";
                const string jsonArrayToken = " json:Array='true' ";
                */
                string soapEnvelopeToken = Environment.GetEnvironmentVariable("soapEnvelopeToken");
                string xmlnsToken = Environment.GetEnvironmentVariable("xmlnsToken");
                string itemToken = Environment.GetEnvironmentVariable("itemToken");
                string jsonArrayToken = Environment.GetEnvironmentVariable("jsonArrayToken");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                requestBody = requestBody.Replace(soapEnvelopeToken, $"{soapEnvelopeToken} {xmlnsToken}");
                requestBody = requestBody.Replace(itemToken, $"{itemToken} {jsonArrayToken}");
                
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(requestBody);
                
                string convertFromXml = JsonConvert.SerializeXmlNode(doc);
                return (ActionResult)new OkObjectResult(convertFromXml);
            }
            catch (Exception ex)
            {
                return (ActionResult)new BadRequestObjectResult(ex);
            }
        }
    }
}