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
        
        public ActionResult HistPriceChart()
        {
            string _quoteNum = "0005.HK";
            YqlHelper _yqlHelper = new YqlHelper();
            return PartialView(_yqlHelper.AnotherHistPrice(_quoteNum, new DateTime(2013, 08, 01), new DateTime(2013, 08, 30)));
        }


    }
}
