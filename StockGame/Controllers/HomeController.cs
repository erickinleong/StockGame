using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using StockGame.Helper;

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
    }
}
