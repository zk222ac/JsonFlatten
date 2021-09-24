using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JsonFlatten
{
    class Program
    {
        static void Main(string[] args)
        {
            // Valid Json Files - Successful Criteria 
            string rawJson1 = @"C:\Users\ajh\Desktop\Task Json Converter\Success Criteria\Raw1.json";
            string rawJson2 = @"C:\Users\ajh\Desktop\Task Json Converter\Success Criteria\Raw2.json";
            string rawJson3 = @"C:\Users\ajh\Desktop\Task Json Converter\Success Criteria\Raw3.json";
            string rawJson4 = @"C:\Users\ajh\Desktop\Task Json Converter\Success Criteria\Raw4.json";
            string rawJson5 = @"C:\Users\ajh\Desktop\Task Json Converter\Success Criteria\Raw5.json";
            string rawJson6 = @"C:\Users\ajh\Desktop\Task Json Converter\Success Criteria\Raw6.json";
            string rawJson7 = @"C:\Users\ajh\Desktop\Task Json Converter\Success Criteria\Raw7_500_records.json";
            string rawJson8 = @"C:\Users\ajh\Desktop\Task Json Converter\Success Criteria\Raw8_1000_records_array.json";
            string rawJson9 = @"C:\Users\ajh\Desktop\Task Json Converter\Success Criteria\Raw9.json";
            string rawJson10 = @"C:\Users\ajh\Desktop\Task Json Converter\Success Criteria\Raw10.json";


            List<string> lsFiles = new List<string>()
            {
               rawJson1,rawJson2,rawJson3,rawJson4,rawJson5,rawJson6,rawJson7,rawJson8,rawJson9,rawJson10
            };

            // Invalid Json Files - Unsuccessful Criteria
            string errorJson1 = @"C:\Users\ajh\Desktop\Task Json Converter\unsuccessful criteria\ErrorRaw1.json";
            string errorJson2 = @"C:\Users\ajh\Desktop\Task Json Converter\unsuccessful criteria\ErrorRaw2.json";
            string errorJson3 = @"C:\Users\ajh\Desktop\Task Json Converter\unsuccessful criteria\ErrorRaw3.json";
            string errorJson4 = @"C:\Users\ajh\Desktop\Task Json Converter\unsuccessful criteria\ErrorRaw4.json";
            string errorJson5 = @"C:\Users\ajh\Desktop\Task Json Converter\unsuccessful criteria\ErrorRaw5.json";
           



            string cJsonString = errorJson5;

            try
            {
                if (File.Exists(cJsonString))
                {
                    // Reading data form File 
                    using var sr = new StreamReader(cJsonString);
                    var rawJsonString = sr.ReadToEnd();
                    // Is  check multiple errors in a json file
                    // Task<bool> task = JsonHelper.IsCheckMultipleErrors(rawJsonString);
                    // Validate json file
                    if (JsonHelper.IsValidJson(rawJsonString) && JsonHelper.IsStrictComplainceParse(rawJsonString))
                    {
                        var flattenTheJsonJsonString = MainFlattenTheJson(rawJsonString);
                       // Console.WriteLine(flattenTheJsonJsonString);
                       // Save data into file 
                      _ = WriteDataIntoFile(flattenTheJsonJsonString);
                    }
                    else
                    {
                        Console.WriteLine("Json String is Invalid");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }

       public static string MainFlattenTheJson(string rawJsonString)
        {
            // flatten the json  
            var oDict = JsonHelper.DeserializeAndFlatten(rawJsonString);
            // Trim Arrays in comma separated values
            var nDict = JsonHelper.TrimArrayValuesInDictionary(oDict);
            Console.WriteLine("..................................................................................................");
            // serialize json file here  
            var translatorJsonString = JsonConvert.SerializeObject(nDict , Formatting.Indented);
            foreach (var kvp in nDict)
            {
                Console.WriteLine(kvp.Key + ": " + kvp.Value);
            }
            return translatorJsonString;
        }

       static async Task WriteDataIntoFile(string jsonString)
       {
           // Set a variable to the Documents path.
           string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

           // Write the specified text asynchronously to a new file named "WriteTextAsync.txt".
           await using StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "Translator.json"));
           await outputFile.WriteAsync(jsonString);
           // : kindly check the translator file in this folder of your computer ( C: \Users\ajh\Documents)
           // ajh --> probably your user name 
       }
    }
}
