using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StockGame.Helper;

namespace StockGame.Controllers
{
    public class StockInfoController : Controller
    {
        //
        // GET: /StockInfo/

        public ActionResult StockHome()
        {

            return View();
        }

        [HttpGet]
        [ChildActionOnly]
        public ActionResult HistPriceChart(string _quoteNum)
        {
            YqlHelper _yqlHelper = new YqlHelper();

            //Build Dataset for Historical Price Chart drawing
            ViewDataDictionary _chartInfo = new ViewDataDictionary();
            _chartInfo.Add(new KeyValuePair<string, object>("Data", _yqlHelper.HistPrice("0005.HK",
                                                                                        new DateTime(2013, 08, 01),
                                                                                        new DateTime(2013, 08, 30))));
            _chartInfo.Add(new KeyValuePair<string, object>("Height", 400));
            _chartInfo.Add(new KeyValuePair<string, object>("Width", 1000));
            _chartInfo.Add(new KeyValuePair<string, object>("CompanyName", _yqlHelper.InstantPrice(_quoteNum).Item1));

            return PartialView("_HistPriceChartPartial", _chartInfo);
        }

        public ActionResult AnotherHistPriceChart()
        {
            string _quoteNum = "0005.HK";
            YqlHelper _yqlHelper = new YqlHelper();
            return View(_yqlHelper.AnotherHistPrice(_quoteNum, new DateTime(2013, 08, 01), new DateTime(2013, 08, 30)));
        }

    }
}
