using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateCharts
{
    public class TradeActions
    {
        public const string Buy = "\"buy\"";
        public const string Sell = "\"sell\"";

        public DateTime Date { get; set; }

        public string ActionName { get; set; }

        public int DataNumber { get; set; }
    }
}
