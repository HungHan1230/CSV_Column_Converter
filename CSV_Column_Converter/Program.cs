using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace CSV_Column_Converter
{
    class Program
    {        
        static void Main(string[] args)
        {
            Console.WriteLine("從Input資料夾讀取資料中...");
            // Get file names at ./Input
            string[] fileEntries = Directory.GetFiles("./Input/");

            // if there is no file in the ./Input, shut down the program.
            if (fileEntries.Length == 0)
            {
                Console.WriteLine("請確認Input資料夾內是否放置檔案！");
            }
            else
            {
                foreach (string filename in fileEntries)
                {
                    WriteOutput(ReadInput(filename), filename);
                }
                //Console.WriteLine(ReadInput().ToArray()[0]["Amount"]); //  00000001963       
            }

            //for (int i = 0; i < 128; i++) {
            //    File.AppendAllText("./Input/PRSBSPC3", "00252759110021000216505160002483+00000001318DPI16100029935\n");
            //}

            Console.WriteLine();
            Console.WriteLine("程式執行結束！按一下Enter即可結束應用程式！");
            while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
        }        
        
        static void WriteOutput(List<Dictionary<string, string>> input, string filename) {            
            int Serial_Number = 1;
            string transaction_date;
            string output = "";
            try
            {
                foreach (Dictionary<string, string> input_entry in input) {
                    StringBuilder output_text = new StringBuilder();
                    output_text.Append(input_entry["User_ID"]).Append(","); //稅單號碼
                    output_text.Append("DP").Append(","); //DP 台中關
                    output_text.Append(Serial_Number++).Append(","); // 每個關區由1開始編號
                    output_text.Append(Convert.ToInt32(input_entry["Amount"])).Append(","); // 最大長度5
                    
                    transaction_date = ChangeDateFormat(input_entry["Transaction_Date"]);
                    output_text.Append(transaction_date).Append(","); // change format of Date yyyy/MM/dd                   
                    output_text.Append(input_entry["User_ID"].StartsWith("DPI") ? "C" : "Q").Append(","); // 判斷C: 現金 或 Q: 電子支付(QR code)
                    output_text.Append(DateTime.Now.ToString("yyyy/MM/dd")).Append("\n"); // 核帳日期yyyy/MM/dd 海關人員預計核帳日期
                    //output_text.Append(Transaction_Action(input_entry["User_ID"])).Append(","); 

                    output = new StringBuilder("./Output/BIP006_400579_")
                        .Append(DateTime.Parse(transaction_date).ToString("yyyyMMdd"))
                        .Append(".csv").ToString();
                    File.AppendAllText(output, output_text.ToString());                    
                }
                Console.WriteLine(output + "寫入完畢！");
            }
            catch (Exception e) {
                Console.WriteLine("Output時發生問題！" + e.Message);
            }
        }

        static string ChangeDateFormat(string date) {
            StringBuilder new_date = new StringBuilder();
            // 是否會有民國100年以前的年份出現? 1100209
            int western_year = Convert.ToInt32(date.Substring(0, 3)) + 1911;
            new_date.Append(western_year).Append("/");
            new_date.Append(date.Substring(3,2)).Append("/");
            new_date.Append(date.Substring(5, 2));
            return new_date.ToString();
        }
        //static string Transaction_Action(string user_id) {
        //    if (user_id.StartsWith("DPI")) return "C";
        //    else return "Q";
        //}

        static List<Dictionary<string, string>> ReadInput(string filename) {
            List<Dictionary<string, string>> input_list = new List<Dictionary<string, string>>();
            Dictionary<string, string> input;
            string line;
            try
            {
                StreamReader file = new StreamReader(filename);
                while ((line = file.ReadLine()) != null) {
                    if (line.Length == 0) break;
                    input = new Dictionary<string, string>();
                    ProcessInput(line, input);
                    input_list.Add(input);
                }
                file.Close();                
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);                
            }
            return input_list;
        }

        static void ProcessInput(string input_str, Dictionary<string,string> input) {            
            input["Transfer_Account"] = input_str.Substring(0,8); // 劃撥帳號
            input["Transaction_Date"] = input_str.Substring(8, 7); // 交易日期
            input["Office_Number"] = input_str.Substring(15, 6); // 經辦局號
            input["Transaction_Code"] = input_str.Substring(21, 4); // 交易代號
            input["Transaction_Number"] = input_str.Substring(25, 7); // 交易序號
            input["Action"] = input_str.Substring(32, 1); // 存提別
            input["Amount"] = input_str.Substring(33, 11); // 交易金額
            input["User_ID"] = input_str.Substring(44,14); // 用戶編號
        }

    }
}
