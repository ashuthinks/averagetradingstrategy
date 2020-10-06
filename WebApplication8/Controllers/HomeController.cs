using Intrinio.SDK.Api;
using Intrinio.SDK.Client;
using Intrinio.SDK.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication8.Models;

namespace WebApplication8.Controllers
{
    public class HomeController : Controller
    {
        public static SecurityApi securityApi = new SecurityApi();
        public ActionResult Index()
        {
            //StockPriceData sp = new StockPriceData();
            //List<StockPriceData> result = new List<StockPriceData>();
            //result.Add(new StockPriceData() { buyprice = 12, highstoploss = 12, lowstoploss = 12, shortprice = 12, stock = "cd" });
            //result.Add(new StockPriceData() { buyprice = 55, highstoploss = 66, lowstoploss = 33, shortprice = 88, stock = "gt" });


            List<StockPriceData> result = ProcessData();
            return View(result);
        }

        public static List<StockPriceData> ProcessData()
        {
            List<StockPriceData> resultData = new List<StockPriceData>();
            string[] stocks = { "EQTCS", "EQSBIN", "EQRELIANCE", "EQMARUTI", "EQKOTAKBANK", "EQICICIBANK", "EQAXISBANK" };

            Configuration.Default.AddApiKey("api_key", "OmY2ODg5ZmFjMWZiNWMxMzM3YTE4OTYyMWMxNzMxMDJh");
            var identifier = "EQTCS";

            // taking current date 
            var endDate = DateTime.Parse(DateTime.Today.ToString("yyyy-MM-dd"));
            var startDate = DateTime.Parse(DateTime.Today.AddDays(-20).ToString("yyyy-MM-dd"));

            // pass dates for back testing
            //var startDate = DateTime.Parse("2020-05-01");
            //var endDate = DateTime.Parse("2020-05-09");

            var frequency = "daily";
            var pageSize = 100;
            var nextPage = "";

            try
            {
                foreach (var item in stocks)
                {
                    identifier = item;
                    ApiResponseSecurityStockPrices result = securityApi.GetSecurityStockPrices(identifier, startDate, endDate, frequency, pageSize, nextPage);
                    var tee = result.StockPrices.Take(10).ToList();
                    ApiResponseSecurityStockPrices result1 = new ApiResponseSecurityStockPrices() { StockPrices = tee };

                    var json = JObject.Parse(result1.ToJson());

                    // get low
                    var properties = json.DescendantsAndSelf()
                        .OfType<JProperty>()
                        .Where(p => p.Name == "low");

                    var lowProperty = properties
                        .Aggregate((p1, p2) => p1.Value.Value<double>() < p2.Value.Value<double>() ? p1 : p2);

                    var highProperty = (lowProperty?.Parent as JObject)?.Property("high");

                    // get high 
                    var propertiesHigh = json.DescendantsAndSelf()
                     .OfType<JProperty>()
                     .Where(p => p.Name == "high");

                    var lowPropertyHigh = propertiesHigh
                        .Aggregate((p1, p2) => p1.Value.Value<double>() > p2.Value.Value<double>() ? p1 : p2);

                    var highPropertyHigh = (lowPropertyHigh?.Parent as JObject)?.Property("low");

                    resultData.Add(new StockPriceData() { buyprice = highProperty.Value.ToString(), lowstoploss = lowProperty.Value.ToString(), shortprice = highPropertyHigh.Value.ToString(), highstoploss = lowPropertyHigh.Value.ToString(), stock = identifier });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception when calling SecurityApi.GetSecurityStockPrices: " + ex.Message);
            }
            return resultData;
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}