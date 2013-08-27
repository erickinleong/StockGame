using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace StockGame.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var _client = new RestClient("http://query.yahooapis.com/v1/public/yql");
            var _request = new RestRequest("?q=select%20*%20from%20yahoo.finance.quotes%20where%20symbol%20in%20(%220001.HK%22)%0A%09%09&format=json&env=http%3A%2F%2Fdatatables.org%2Falltables.env", Method.GET);
            var _result = _client.Execute(_request).Content;

            JObject _block = JsonConvert.DeserializeObject<JObject>(_result);
            var _query = JsonConvert.DeserializeObject<JObject>(_block.Property("query").Value.ToString());
            var _results = JsonConvert.DeserializeObject<JObject>(_query.Property("results").Value.ToString());
            var _stocks = JsonConvert.DeserializeObject<JObject>(_results.Property("quote").Value.ToString());


            ViewBag.Message = _stocks.Property("Ask").Value.ToString();

            //ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";


            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
