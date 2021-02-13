using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace CSV_Column_Converter
{
    class Program
    {
        static Dictionary<string, string> config ;
        static void Main(string[] args)
        {
            //TestGetAndAddDirectory();
            //ReadCurrentDirectory();

            // Read the config file the get the target file name and the name of the output file.
            // Config includes "Input_File_Name" and "Output_FileName".
            config = ReadConfig();

            Process(ReadCSV(config));

            Console.WriteLine();
            Console.WriteLine("程式執行結束！按一下Enter即可結束應用程式！");
            while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }


        }

        //show the columns and select the ones you want to keep and discard the other columns.
        static void Process(string[] columns) {           

            try
            {                
                ConsoleKeyInfo keyInfo;
                string[] usr_columns = null;
                do
                {
                    Boolean isValid = false;                   

                    Console.WriteLine("以下為{0}中的欄位，請選擇要輸出的欄位。\n例如：要輸出：「{1}」和「{2}」，輸入「1 2」即可", config["Input_File_Name"], columns[0], columns[1]);

                    int index = 0;
                    foreach (string column in columns)
                    {
                        Console.WriteLine("{0}. {1}", ++index, column);
                    }
                    Console.WriteLine();


                    while (!isValid)
                    {
                        Console.Write("請輸入選取的欄位：");
                        string columns_str = Console.ReadLine();
                        usr_columns = columns_str.Split(' ');
                        
                        foreach (string column in usr_columns)
                        {
                            int i = Convert.ToInt32(column);
                            if (i > columns.Length)
                                Console.WriteLine("輸入數字大於欄位數量！請重新輸入！");
                            else
                                isValid = true;
                        }
                    }
                    Console.WriteLine("您選擇的是：");
                    foreach (string usr_column in usr_columns)
                        Console.WriteLine("{0}. {1}", Convert.ToInt32(usr_column), columns[Convert.ToInt32(usr_column) - 1]);

                    Console.Write("請問確定以『 ");//{0}, {1}輸出嗎?", Convert.ToInt32(usr_column), columns[Convert.ToInt32(usr_column) - 1]);
                    foreach (string usr_column in usr_columns)
                        Console.Write("{0} ", columns[Convert.ToInt32(usr_column) - 1]);
                    Console.Write("』這個順序來輸出成{0}嗎？(Y/N):", config["Output_File_Name"]);                    
                    keyInfo = Console.ReadKey();                    
                    Console.WriteLine();

                } while ( keyInfo.Key.ToString().StartsWith("N") || keyInfo.Key.ToString().StartsWith("n"));
                

                WriteCSV(usr_columns);

            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine("請確實輸入指定的格式！");
            }
            
        }
        static void WriteCSV(string[] usr_columns) {
            Console.WriteLine("寫入{0}中", config["Output_File_Name"]);
            string line;
            string[] columns;
            try
            {                
                StreamReader file = new StreamReader("./Data/" + config["Input_File_Name"]);
                while ((line = file.ReadLine()) != null)
                {                    
                    columns = line.Split(',');
                    string output_text = "";
                    foreach (string usr_column in usr_columns)                                            
                        output_text += columns[Convert.ToInt32(usr_column) - 1] + ",";

                    //Console.WriteLine(output_text);
                    File.AppendAllText("./Data/" + config["Output_File_Name"], output_text + "\n");

                }

                file.Close();                

            }
            catch (Exception e)
            {
                Console.WriteLine("{0} 寫入失敗！\n{0}", config["Output_File_Name"], e.Message);                
            }           

        }       

        static string[] ReadCSV(Dictionary<string,string> config) {
            string line;
            int counter = 0;
            string[] columns = null;

            //Read csv file and return the columns
            try
            {                
                //StreamReader file = new StreamReader(@"c:\test.txt");
                StreamReader file = new StreamReader("./Data/"+config["Input_File_Name"]);
                
                while ((line = file.ReadLine()) != null)
                {
                    //Console.WriteLine(line);
                    columns = line.Split(',');
                    counter++;
                    if (counter == 1)
                        break;
                }

                file.Close();
                //Console.WriteLine("There were {0} lines.", counter);
                // Suspend the screen.  
                //System.Console.ReadLine();

            }
            catch (Exception e) {
                Console.WriteLine("The csv file could not be read!");
                Console.WriteLine(e.Message);
            }

            return columns;

            
        }
        

        static Dictionary<string,string> ReadConfig() {
            Dictionary<string, string> output = null;
            try
            {
                string jsonString = File.ReadAllText("./Config.json");
                output = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
            }
            catch (Exception e) {
                Console.WriteLine("請確認Config.json是否在此資料夾！\n{0}",e.Message);                
            }
            

            //if config.json not exists, create one.
            //checkConfigJson();
            return output;

            //return JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("./Config.json"));
        }

       

        //public static void checkConfigJson() {
        //    string path = Directory.GetCurrentDirectory();
        //    //Console.WriteLine("The current directory is {0}", path);
        //    ProcessDirectory(path);
        //}

        //// Process all files in the directory passed in, recurse on any directories
        //// that are found, and process the files they contain.
        //public static void ProcessDirectory(string targetDirectory)
        //{
        //    // Process the list of files found in the directory.
        //    string[] fileEntries = Directory.GetFiles(targetDirectory);
        //    Boolean ConfigIsExists = false;
        //    foreach (string fileName in fileEntries) {
        //        if (fileName.Equals("Config.json")) {
        //            ConfigIsExists = true;
        //            break;
        //        }
        //    }

        //    if (ConfigIsExists)
        //    {


        //    }
        //    else {
        //        Console.WriteLine("建立Config.json");
        //    }

            

        //    //foreach (string fileName in fileEntries)
        //    //    ProcessFile(fileName);

        //        // Recurse into subdirectories of this directory.
        //        //string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
        //        //foreach (string subdirectory in subdirectoryEntries)
        //        //    ProcessDirectory(subdirectory);
        //}

        //// Insert logic for processing found files here.
        //public static void ProcessFile(string path)
        //{
        //    Console.WriteLine("Processed file '{0}'.", path);
        //}

        //static void TestGetAndAddDirectory() {
        //    try
        //    {
        //        // Get the current directory.
        //        string path = Directory.GetCurrentDirectory();
        //        string target = @"c:\temp";
        //        Console.WriteLine("The current directory is {0}", path);
        //        if (!Directory.Exists(target))
        //        {
        //            Directory.CreateDirectory(target);
        //        }

        //        // Change the current directory.
        //        Environment.CurrentDirectory = (target);
        //        if (path.Equals(Directory.GetCurrentDirectory()))
        //        {
        //            Console.WriteLine("You are in the temp directory.");
        //        }
        //        else
        //        {
        //            Console.WriteLine("You are not in the temp directory.");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("The process failed: {0}", e.ToString());
        //    }
        //}
    }
}
