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
using RestSharp;

namespace StockGame.Helper
{
    public class YqlHelper
    {
        public InstantStockModel InstantPrice(String quote)
        {
            //Build Yahoo Query request address
            StringBuilder _webAddress = new StringBuilder();

            _webAddress.Append("http://finance.yahoo.com/d/quotes.csv?s=" + quote + "&f=snbaopl1");

            InstantStockModel _yqlresult = _instStockDataConverter(new WebClient().DownloadString(_webAddress.ToString()));

            return _yqlresult;
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

            _webAddress.Append("http://ichart.finance.yahoo.com/table.csv?s=" + quote);
            _webAddress.Append("&d=" + (endDate.Month - 1));
            _webAddress.Append("&e=" + (endDate.Day));
            _webAddress.Append("&f=" + (endDate.Year));
            _webAddress.Append("&g=d&a=" + (startDate.Month - 1));
            _webAddress.Append("&b=" + (startDate.Day));
            _webAddress.Append("&c=" + (startDate.Year));
            _webAddress.Append("&ignore=.csv");

            List<HistoricalStockModel> _histStockPriceList = _histStockDataConverter(new WebClient().DownloadString(_webAddress.ToString()));

            Dictionary<DateTime, double> _chartData = new Dictionary<DateTime, double>();

            foreach (var _hist in _histStockPriceList) 
            {
                _chartData.Add(_hist.Date, _hist.High); 
            }


            object[,] chartData = new object[_chartData.Count, 2];
            int i = 0;
            foreach (KeyValuePair<DateTime, double> pair in _chartData)
            {
                chartData.SetValue(pair.Key, i, 0);
                chartData.SetValue(pair.Value, i, 1);
                i++;
            }

            Highcharts chart1 = new Highcharts("chart1")
                .InitChart(new Chart { Type = ChartTypes.Line })
                .SetTitle(new Title { Text = InstantPrice("0005.HK").Name, Style = "color: '#999',fontWeight: 'bold', fontSize: 'large'" })
                .SetXAxis(new XAxis { Type = AxisTypes.Datetime })
                .SetSeries(new Series { Data = new Data(chartData), Name = "Price" });

            return chart1;
        }

        private InstantStockModel _instStockDataConverter(string data)
        {
            InstantStockModel prices = new InstantStockModel();

            string[] rows = data.Replace("\r", "").Split('\n');

            foreach (string row in rows)
            {
                if (string.IsNullOrEmpty(row)) continue;

                string[] cols = row.Split(',');

                //InstantStockModel p = new InstantStockModel();
                prices.Symbol = cols[0];
                prices.Name = cols[1];
                prices.Name = prices.Name.Replace("\"", "");
                //p.Bid = Convert.ToDecimal(cols[2]);
                prices.Ask = Convert.ToDecimal(cols[3]);
                prices.Open = Convert.ToDecimal(cols[4]);
                prices.PreviousClose = Convert.ToDecimal(cols[5]);
                prices.Last = Convert.ToDecimal(cols[6]);

                //prices.Add(p);
            }

            return prices;
        }

        private List<HistoricalStockModel> _histStockDataConverter(string data)
        {
            List<HistoricalStockModel> retval = new List<HistoricalStockModel>();

            data = data.Replace("r", "");

            string[] rows = data.Split('\n');

            //First row is headers so Ignore it
            for (int i = 1; i < rows.Length; i++)
            {
                if (rows[i].Replace("\n", "").Trim() == "") continue;

                string[] cols = rows[i].Split(',');

                HistoricalStockModel hs = new HistoricalStockModel();
                hs.Date = Convert.ToDateTime(cols[0]);
                hs.Open = Convert.ToDouble(cols[1]);
                hs.High = Convert.ToDouble(cols[2]);
                hs.Low = Convert.ToDouble(cols[3]);
                hs.Close = Convert.ToDouble(cols[4]);
                hs.Volume = Convert.ToDouble(cols[5]);
                hs.AdjClose = Convert.ToDouble(cols[6]);

                retval.Add(hs);
            }

            return retval;
            
        }

        private string _YqlBaseAddress = "http://query.yahooapis.com/v1/public/yql";
        private string _YqlRequestAttribute = "&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=";
    }

}