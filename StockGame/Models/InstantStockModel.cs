using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StockGame.Models
{
    public class InstantStockModel
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        //public decimal Bid { get; set; }
        public decimal Ask { get; set; }
        public decimal Open { get; set; }
        public decimal PreviousClose { get; set; }
        public decimal Last { get; set; }
    }
}