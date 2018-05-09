using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;
using System.Linq;

namespace GenerateCharts
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine(@" date: data[{0}].date, type: {1}, price: data[{0}].low, low: data[{0}].low, high: data[{0}].high ,", 1, 2);
            //Console.WriteLine(string.Format(@"\{ date\: data\[{0}\]\.date, type: {1}, price: data[{0}].low, low: data[{0}].low, high: data[{0}].high \},", 1, "123"));
            //return;

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = "78stk";
            List<ChartData> chartDataList = new List<ChartData>();

            if (dbCon.IsConnect())
            {
                //suppose col0 and col1 are defined as VARCHAR in the DB
                string query = "SELECT date,open,close,high,low,volume,code FROM 000002_d_k_data ORDER BY date desc";
                var cmd = new MySqlCommand(query, dbCon.Connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ChartData data = new ChartData
                    {
                        Date = DateTime.Parse(reader.GetString(0)),
                        Open = reader.GetFloat(1),
                        Close = reader.GetFloat(2),
                        High = reader.GetFloat(3),
                        Low = reader.GetFloat(4),
                        Volume = reader.GetInt32(5),
                        Code = reader.GetString(6)
                    };

                    chartDataList.Add(data);
                }
                dbCon.Close();
            }

            TradeDataAnalyzer TradeDataAnalyzer = new TradeDataAnalyzer(chartDataList);
            TradeDataAnalyzer.GenerateDataCsvFile();
            TradeDataAnalyzer.GenerateHtmlFile();
        }
    }
}
