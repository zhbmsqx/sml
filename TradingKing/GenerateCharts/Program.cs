using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;

namespace GenerateCharts
{
    class Program
    {
        static string outputPath = @"D:\78stk\sourceCode\StocksGenerated\{0}";

        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = "78stk";
            List<ChartData> chartDataList = new List<ChartData>();

            if (dbCon.IsConnect())
            {
                //suppose col0 and col1 are defined as VARCHAR in the DB
                string query = "SELECT date,open,close,high,low,volume FROM 000002_d_k_data ORDER BY date desc";
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
                    };

                    chartDataList.Add(data);
                }
                dbCon.Close();
            }

            using (StreamWriter sw = new StreamWriter(string.Format(outputPath, "data.csv")))
            {
                sw.AutoFlush = true;

                sw.WriteLine("Date,Open,High,Low,Close,Volume");

                foreach (var d in chartDataList)
                {
                    sw.WriteLine(d.ToString());
                }
            }
        }
    }
}
