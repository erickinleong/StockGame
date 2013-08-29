using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using StockGame.Helper;
using System.Web.Helpers;

namespace StockGame.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            Tuple<string, string> _quote = YqlHelper.InstantPrice("0005.HK");

            ViewBag.Message = _quote.Item1 + " " + _quote.Item2;
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

        public ActionResult GetChartImage()
        {
            var key = new Chart(width: 300, height: 300, theme: ChartTheme.Blue)
                                .AddTitle("Employee Chart")
                                .AddSeries(
                                chartType: "Line",
                                name: "Employee",
                                xValue: new[] { "Peter", "Andrew", "Julie", "Dave" },
                                yValues: new[] { "2", "7", "5", "3" });

            return File(key.ToWebImage().GetBytes(), "image/jpeg");
        }
    }
}
