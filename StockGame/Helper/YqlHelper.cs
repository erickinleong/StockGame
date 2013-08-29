using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StockGame.Helper
{
    public static class YqlHelper
    {
        public static Tuple<string, string> InstantPrice(String quote)
        {
            var _client = new RestClient("http://query.yahooapis.com/v1/public/yql");
            var _request = new RestRequest("?q=select%20*%20from%20yahoo.finance.quotes%20where%20symbol%20in%20(%220001.HK%22)%0A%09%09&format=json&env=http%3A%2F%2Fdatatables.org%2Falltables.env", Method.GET);
            var _result = _client.Execute(_request).Content;

            JObject _block = JsonConvert.DeserializeObject<JObject>(_result);
            var _query = JsonConvert.DeserializeObject<JObject>(_block.Property("query").Value.ToString());
            var _results = JsonConvert.DeserializeObject<JObject>(_query.Property("results").Value.ToString());
            var _stocks = JsonConvert.DeserializeObject<JObject>(_results.Property("quote").Value.ToString());


            return new Tuple<string, string>(_stocks.Property("symbol").Value.ToString(),
                                             _stocks.Property("Ask").Value.ToString());
        }


    }
}