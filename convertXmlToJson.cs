using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Schema;

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
            try
            {
                /* Use this block to pull token values from environment variables
                string soapEnvNsToken = Environment.GetEnvironmentVariable("soapEnvNsToken");
                string jsonNsToken = Environment.GetEnvironmentVariable("jsonNsToken");
                string lineItemNsToken = Environment.GetEnvironmentVariable("lineItemNsToken");
                string xPathQuery = Environment.GetEnvironmentVariable("xPathQuery");
                */
                const string soapEnvNsToken = "http://schemas.xmlsoap.org/soap/envelope/";
                const string jsonNsToken = "http://james.newtonking.com/projects/json";
                const string lineItemNsToken = "http://www.yantra.com/xapidocs/pricingservice/pricingservicerequest";
                const string xPathQuery = "/soapenv:Envelope/soapenv:Body/yfc:ItemList/yfc:Item";
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                //Load request body into an XmlDocument
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(requestBody);
                           
                //Add the namespace to the manager
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("soapenv", soapEnvNsToken);
                nsmgr.AddNamespace("yfc", lineItemNsToken);
                //Search for the items nodes
                XmlNodeList items = doc.SelectNodes(xPathQuery, nsmgr);
                //If there is only one item, add the json:Array attribute
                if (items != null && items.Count == 1)
                {
                    XmlAttribute jsonArrayAttribute = doc.CreateAttribute("json", "Array", jsonNsToken);
                    jsonArrayAttribute.Value = "true";
                    doc.DocumentElement.SetAttribute("xmlns:json", jsonNsToken);
                    items[0].Attributes.SetNamedItem(jsonArrayAttribute);
                }

                //Serialize the Xml to JSON
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