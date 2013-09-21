using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Helpers;
using System.Text;
using System.Net;
using StockGame.Models;
using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using Point = DotNet.Highcharts.Options.Point;
using Chart = DotNet.Highcharts.Options.Chart;

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

                _jsonResult = _jsonResult + "[" + DateTimeHelper.ToJsonTicks(_date) + "," + q["High"] + "],";
            }
            _jsonResult = _jsonResult.Remove(_jsonResult.Length - 1);
            _jsonResult = _jsonResult + "] }";

            return _jsonResult;
        }

        public Highcharts AnotherHistPrice(string quote, DateTime startDate, DateTime endDate)
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

            Dictionary<DateTime, double> data = new Dictionary<DateTime, double>();

            foreach (var _jItem in _jArray) 
            {
                data.Add(DateTime.Parse(_jItem["date"].ToString()), Double.Parse(_jItem["High"].ToString())); 
            }


            object[,] chartData = new object[data.Count, 2];
            int i = 0;
            foreach (KeyValuePair<DateTime, double> pair in data)
            {
                chartData.SetValue(pair.Key, i, 0);
                chartData.SetValue(pair.Value, i, 1);
                i++;
            }

            Highcharts chart1 = new Highcharts("chart1")
                .InitChart(new Chart { Type = ChartTypes.Line })
                .SetTitle(new Title { Text = "Chart 1" })
                .SetXAxis(new XAxis { Type = AxisTypes.Datetime })
                .SetSeries(new Series { Data = new Data(chartData), Name = "Price" });

            return chart1;
        }

        private string _YqlBaseAddress = "http://query.yahooapis.com/v1/public/yql";
        private string _YqlRequestAttribute = "&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=";
    }

}