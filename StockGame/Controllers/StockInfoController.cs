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
        
        public ActionResult HistPriceChart(string _quoteNum)
        {
            //string _quoteNum = "0005.HK";
            YqlHelper _yqlHelper = new YqlHelper();
            return PartialView(_yqlHelper.HistPriceChart(_quoteNum, new DateTime(2013, 09, 01), new DateTime(2013, 09, 30)));
        }


    }
}
