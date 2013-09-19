using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Helpers;
using System.Text;

namespace StockGame.Helper
{
    public class YqlHelper
    {
        public Tuple<string, string> InstantPrice(String quote)
        {
            var _client = new RestClient(_YqlBaseAddress);

            StringBuilder _webAddress = new StringBuilder();
            _webAddress.Append("?q=" + System.Web.HttpUtility.UrlEncode("SELECT * FROM yahoo.finance.oquote WHERE symbol="));
            _webAddress.Append("'" + quote + "'");
            _webAddress.Append("&format=json");
            _webAddress.Append("&env=http%3A%2F%2Fdatatables.org%2Falltables.env&callback=");

            var _result = _client.Execute(new RestRequest(_webAddress.ToString(), Method.GET)).Content;

            JObject _resultObject = (JObject)JObject.Parse(_result)["query"]["results"]["option"];

            return new Tuple<string, string>(_resultObject.Property("sym").Value.ToString(),
                                             _resultObject.Property("price").Value.ToString());
        }

        public string HistPrice(string quote, DateTime startDate, DateTime endDate)
        {
            var _client = new RestClient(_YqlBaseAddress);

            StringBuilder _webAddress = new StringBuilder();
            _webAddress.Append("?q=" + System.Web.HttpUtility.UrlEncode("select * from yahoo.finance.historicaldata where symbol = "));
            _webAddress.Append("'" + quote + "'");
            _webAddress.Append("and startDate = '" + startDate.ToString("yyyy-MM-dd") + "'");
            _webAddress.Append("and endDate = '" + endDate.ToString("yyyy-MM-dd") + "'");
            _webAddress.Append("&format=json");
            _webAddress.Append("&env=http%3A%2F%2Fdatatables.org%2Falltables.env&callback=");

            string _yqlResult = _client.Execute(new RestRequest(_webAddress.ToString(), Method.GET)).Content;

            JArray _jArray = (JArray)JObject.Parse(_yqlResult)["query"]["results"]["quote"];

            string _result = "{data: [";
            foreach (var q in _jArray)
            {
                DateTime _date = DateTime.Parse(q["date"].ToString());
                _result = _result + "[" + _date.Day.ToString() + "," + q["High"] + "],";
            }
            _result = _result.Remove(_result.Length - 1);
            _result = _result + "] }";

            return _result;
        }

        private string _YqlBaseAddress = "http://query.yahooapis.com/v1/public/yql";
    }
}