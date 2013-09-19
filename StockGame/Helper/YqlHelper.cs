using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Helpers;
using System.Text;
using System.Net;

namespace StockGame.Helper
{
    public class YqlHelper
    {
        public Tuple<string, string> InstantPrice(String quote)
        {
            //Build Yahoo Query request address
            StringBuilder _webAddress = new StringBuilder();
            _webAddress.Append(_YqlBaseAddress);
            _webAddress.Append("?q=" + "SELECT%20*%20FROM%20yahoo.finance.quote%20WHERE%20symbol%3D");
            _webAddress.Append("%22" + quote + "%22");
            _webAddress.Append(_YqlRequestAttribute);


            string _yqlresult = new WebClient().DownloadString(_webAddress.ToString());

            JObject _resultObject = (JObject)JObject.Parse(_yqlresult)["query"]["results"]["quote"];

            return new Tuple<string, string>(_resultObject.Property("Name").Value.ToString(),
                                             _resultObject.Property("DaysHigh").Value.ToString());
        }

        public string HistPrice(string quote, DateTime startDate, DateTime endDate)
        {
            //Build Yahoo Query request address
            StringBuilder _webAddress = new StringBuilder();
            _webAddress.Append(_YqlBaseAddress);
            _webAddress.Append("?q=" + "SELECT%20*%20FROM%20yahoo.finance.historicaldata%20WHERE%20symbol%3D");
            _webAddress.Append("%22" + quote + "%22");
            _webAddress.Append("and%20startDate=%22" + startDate.ToString("yyyy-MM-dd") + "%22");
            _webAddress.Append("and%20endDate=%22" + endDate.ToString("yyyy-MM-dd") + "%22");
            _webAddress.Append(_YqlRequestAttribute);

            string _yqlResult = new WebClient().DownloadString(_webAddress.ToString());

            JArray _jArray = (JArray)JObject.Parse(_yqlResult)["query"]["results"]["quote"];

            string _jsonResult = "{data: [";
            foreach (var q in _jArray)
            {
                DateTime _date = DateTime.Parse(q["date"].ToString());
                _jsonResult = _jsonResult + "[" + _date.Day.ToString() + "," + q["High"] + "],";
            }
            _jsonResult = _jsonResult.Remove(_jsonResult.Length - 1);
            _jsonResult = _jsonResult + "] }";

            return _jsonResult;
        }

        private string _YqlBaseAddress = "http://query.yahooapis.com/v1/public/yql";
        private string _YqlRequestAttribute = "&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=";
    }
}