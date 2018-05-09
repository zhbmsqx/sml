using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateCharts
{
    public class ChartData
    {
        public DateTime Date { get; set; }

        public float Open { get; set; }

        public float Close { get; set; }

        public float High { get; set; }

        public float Low { get; set; }

        public long Volume { get; set; }

        public string Code { get; set; }

        public override string ToString()
        {
            string formatedDate = Date.ToString("d-MMM-y");
            return string.Format("{0},{1},{2},{3},{4},{5}", formatedDate, Open.ToString("0.00"), High.ToString("0.00"), Low.ToString("0.00"), Close.ToString("0.00"), Volume);
        }
    }
}
