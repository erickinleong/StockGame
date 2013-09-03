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
            var _client = new RestClient("http://query.yahooapis.com/v1/public/yql");
            var _request = new RestRequest("?q=select%20*%20from%20yahoo.finance.quotes%20where%20symbol%20in%20(%22" + 
                                           quote +
                                           "%22)%0A%09%09&format=json&env=http%3A%2F%2Fdatatables.org%2Falltables.env", Method.GET);
            var _result = _client.Execute(_request).Content;

            JObject _block = JsonConvert.DeserializeObject<JObject>(_result);
            var _query = JsonConvert.DeserializeObject<JObject>(_block.Property("query").Value.ToString());
            var _results = JsonConvert.DeserializeObject<JObject>(_query.Property("results").Value.ToString());
            var _stocks = JsonConvert.DeserializeObject<JObject>(_results.Property("quote").Value.ToString());


            return new Tuple<string, string>(_stocks.Property("Name").Value.ToString(),
                                             _stocks.Property("Ask").Value.ToString());
        }

        public string HistPrice(string quote, DateTime startDate, DateTime endDate)
        {
            var _client = new RestClient("http://query.yahooapis.com/v1/public/yql");
            string _startDateString = startDate.ToString("yyyy-MM-dd");
            string _endDateString = endDate.ToString("yyyy-MM-dd");

            StringBuilder _webAddress = new StringBuilder();
            _webAddress.Append("?q=" + System.Web.HttpUtility.UrlEncode("select * from yahoo.finance.historicaldata where symbol = "));
            _webAddress.Append("'" + quote + "'");
            _webAddress.Append("and startDate = '" + _startDateString + "'");
            _webAddress.Append("and endDate = '" + _endDateString + "'");
            _webAddress.Append("&format=json");
            _webAddress.Append("&env=http%3A%2F%2Fdatatables.org%2Falltables.env&callback=");

            string _result = _client.Execute(new RestRequest(_webAddress.ToString(), Method.GET)).Content;

            JObject _jObject = JObject.Parse(_result);
            JArray _jArray = (JArray)_jObject["query"]["results"]["quote"];

            _result = "{data: [";
            foreach (var q in _jArray)
            {
                DateTime d = DateTime.Parse(q["date"].ToString());
                string _d = JsonConvert.SerializeObject(d, new JsonSerializerSettings()
                                                        {
                                                            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
                                                        });
                
                _result = _result + "[" + d.Day.ToString() + "," + q["High"] + "],";
            }
            _result = _result.Remove(_result.Length - 1);
            _result = _result + "] }";



            return _result;
        }
    }
}