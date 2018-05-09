using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateCharts
{
    public class TradeDataAnalyzer
    {
        private string outputPath = @"C:\Users\Lingqian\Documents\GitHub\sml\HaiboProjectX\TradingKing\WebSite\StockCharts\{0}";

        public List<ChartData> chartDataList { get; set; }

        public ChartData[] chartDataArray { get; set; }

        public string chartCode { get; set; }

        public TradeDataAnalyzer(List<ChartData> chartDataList)
        {
            if (chartDataList == null || chartDataList.Count == 0)
            {
                throw new Exception("chartDataList can't be null");
            }

            this.chartDataList = chartDataList;

            chartDataList.Reverse();
            this.chartDataArray = chartDataList.ToArray();

            this.chartCode = chartDataList.First().Code;
        }

        public void GenerateDataCsvFile()
        {
            // Firstly generate the data file
            using (StreamWriter sw = new StreamWriter(string.Format(outputPath, string.Format("{0}.csv", chartDataList.First().Code))))
            {
                sw.AutoFlush = true;

                sw.WriteLine("Date,Open,High,Low,Close,Volume");

                foreach (var d in chartDataList)
                {
                    sw.WriteLine(d.ToString());
                }
            }
        }

        public void GenerateHtmlFile()
        {
            List<TradeActions> tradeActionsList = new List<TradeActions>();

            float[] sma5 = new float[chartDataArray.Length];
            float[] sma18 = new float[chartDataArray.Length];
            //float[] sma60 = new float[chartDataArray.Length];

            for (int i = 0; i < chartDataArray.Length; i++)
            {
                if (i < 5)
                {
                    sma5[i] = -1;
                    continue;
                }

                sma5[i] = GetSMA(chartDataList, i, 5);

                if (i < 18)
                {
                    sma18[i] = -1;
                    continue;
                }

                sma18[i] = GetSMA(chartDataList, i, 18);

                if (sma5[i] > sma18[i]
                    && sma5[i - 1] > 0 && sma5[i] > sma5[i - 1])
                {
                    tradeActionsList.Add(new TradeActions
                    {
                        Date = chartDataList[i].Date,
                        ActionName = TradeActions.Buy,
                        DataNumber = i
                    });
                }
                else if (sma5[i] < sma18[i]
                    && sma5[i-1] > 0 && sma5[i] < sma5[i - 1])
                {
                    tradeActionsList.Add(new TradeActions
                    {
                        Date = chartDataList[i].Date,
                        ActionName = TradeActions.Sell,
                        DataNumber = i
                    });
                }
            }

            bool generateTradeActionList = tradeActionsList != null && tradeActionsList.Count != 0;

            using (StreamReader sr = new StreamReader(@"C:\Users\Lingqian\Documents\GitHub\sml\HaiboProjectX\TradingKing\GenerateCharts\HtmlGenerator\HtmlBase.html"))
            {
                using (StreamWriter sw = new StreamWriter(string.Format(outputPath, this.chartCode+".html")))
                {
                    sw.AutoFlush = true;

                    string wholeLine = sr.ReadLine();
                    //sw.WriteLine(wholeLine);

                    while (!wholeLine.Contains("FileEND"))
                    {
                        if (generateTradeActionList && wholeLine.Contains("StartTrades"))
                        {
                            sw.WriteLine("var trades = [");

                            int listTotal = 0;
                            foreach (var tradeAction in tradeActionsList)
                            {
                                sw.Write("{" + string.Format(@"date: data[{0}].date, type: {1}, price: data[{0}].low, low: data[{0}].low, high: data[{0}].high", tradeAction.DataNumber, tradeAction.ActionName) + "}");

                                //if (listTotal > 10)
                                //{
                                //    break;
                                //}

                                if (listTotal++ < tradeActionsList.Count()-1)
                                {
                                    sw.WriteLine(",");
                                }
                                else
                                {
                                    sw.WriteLine();
                                }

                                
                            }

                            sw.WriteLine("];");
                        }
                        else if (generateTradeActionList && wholeLine.Contains("SVGTradeArrow1"))
                        {
                            sw.WriteLine("svg.select(\"g.tradearrow\").datum(trades).call(tradearrow);");
                        }
                        else if (generateTradeActionList && wholeLine.Contains("StartSVGTradeArrowCall"))
                        {
                            sw.WriteLine("svg.select(\"g.tradearrow\").call(tradearrow.refresh);");
                        }
                        else if(wholeLine.Contains("SetDataFile"))
                        {
                            sw.WriteLine(string.Format(@"d3.csv({0}, function(error, data) ", "\"" + this.chartCode + ".csv\"") + "{");
                        }
                        else
                        {
                            sw.WriteLine(wholeLine);
                        }
                        
                        wholeLine = sr.ReadLine();
                    }
                }
            }
        }

        private float GetSMA(List<ChartData> chartDataList, int startPosition, int kAverage)
        {
            float sum = 0;

            for (int i = startPosition -kAverage + 1; i<=startPosition; i++)
            {
                sum += chartDataList[i].Close;
            }

            return sum / kAverage;
        }
    }
}
