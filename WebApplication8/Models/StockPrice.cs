using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication8.Models
{
    public class StockPriceData
    {
        public string stock { get; set; }
        public string lowstoploss { get; set; }
        public string buyprice { get; set; }
        public string highstoploss { get; set; }
        public string shortprice { get; set; }
    }
}