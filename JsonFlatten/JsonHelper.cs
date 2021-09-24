using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using NJsonSchema.CodeGeneration.CSharp;
using JsonSchema = NJsonSchema.JsonSchema;

namespace JsonFlatten
{
    public class JsonHelper
    {
        public static Dictionary<string, object> DeserializeAndFlatten(string json)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            JToken token = JToken.Parse(json);
            FillDictionaryFromJToken(dict, token, "");
            return dict;
        }

        private static void FillDictionaryFromJToken(Dictionary<string, object> dict, JToken token, string prefix)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    foreach (JProperty prop in token.Children<JProperty>())
                    {
                        FillDictionaryFromJToken(dict, prop.Value, Join(prefix, prop.Name));
                    }
                    break;

                case JTokenType.Array:
                    int index = 1;
                    foreach (JToken value in token.Children())
                    {
                        string nPrefix = Join(prefix, index.ToString());
                        FillDictionaryFromJToken(dict, value, nPrefix);
                        index++;
                    }
                    break;

                default:
                    dict.Add(prefix, ((JValue)token).Value);
                    break;
            }
        }

        private static string Join(string prefix, string name)
        {
            return (string.IsNullOrEmpty(prefix) ? name : prefix + "_" + name);
        }

        public static Dictionary<string, object> TrimArrayValuesInDictionary(Dictionary<string, object> oDictioanry)
        {
            StringBuilder sbValue = new StringBuilder();
            Dictionary<string, object> nDictionary = new Dictionary<string, object>();
            for (int i = 0; i < oDictioanry.Count - 1; i++)
            {
                var currentElement = oDictioanry.ElementAt(i);
                var nextElement = oDictioanry.ElementAt(i + 1);

                var cKey = currentElement.Key;
                var cValue = currentElement.Value;
                var nKey = nextElement.Key;
                var nValue = nextElement.Value;
                // check if key has a last character is Digit
                if (char.IsDigit(cKey[^1]) && cKey[^2] == '_')
                {
                    if (char.IsDigit(cKey[^1]) && char.IsDigit(nKey[^1]) && cKey[^2] == '_' && nKey[^2] == '_')
                    {
                        sbValue.Append(cValue + ",");
                        if (!oDictioanry.ContainsKey(cKey)) continue;
                        oDictioanry.Remove(cKey);
                        i--;
                    }
                    else
                    {
                        sbValue.Append(cValue);
                        var mKey = cKey[..^2];
                        nDictionary.TryAdd(mKey, sbValue.ToString());
                        sbValue.Clear();
                    }
                }
                else
                {
                    nDictionary.TryAdd(cKey, cValue);
                }

            }
            return nDictionary;
        }


        public static bool IsValidJson(string input)
        {
            input = input.Trim();
            try
            {
                // check if input is not null
                if (string.IsNullOrWhiteSpace(input)) { return false; }
                if ((input.StartsWith("{") && input.EndsWith("}")) || (input.StartsWith("[") && input.EndsWith("]"))) 
                {
                    try
                    {
                        // Parse as a Token
                        var jToken = JToken.Parse(input);
                        if (jToken.Type == JTokenType.Object)
                        {
                            //parse the input into a JObject
                            var jObject = JObject.Parse(input);
                            foreach (var jo in jObject)
                            {
                                var name = jo.Key;
                                JToken value = jo.Value;
                                switch (value)
                                {
                                    //if the element has a missing value, it will be Undefined - this is invalid
                                    case {Type: JTokenType.Undefined}:
                                        Console.WriteLine($"Undefined Value is not acceptable - {name}-{value}");
                                        return false;
                                    case {Type: JTokenType.Null}:
                                        Console.WriteLine($"Null Value is not acceptable- - {name}-{value}");
                                        return false;
                                }
                            }
                        }

                    }
                    catch (JsonReaderException e)
                    {
                        Console.WriteLine(e.Message);
                        return false;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        return false;
                    }
                }
                else
                {
                    return false;
                }
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool IsStrictComplainceParse(string json)
        {
            try
            {
                // Throw an exception if the json string is not in strict compliance with the JSON standard by tokenize it with the JSON reader used by DataContractJsonSerializer:
                using var stream = GenerateStreamFromString(json);
                using var reader = JsonReaderWriterFactory.CreateJsonReader(stream, XmlDictionaryReaderQuotas.Max);
                while (reader.Read())
                {
                }
            }
            catch (Exception ex)
            {
                // Wrap the XmlException in a JsonReaderException
                throw new JsonReaderException($"Invalid property identifier character is not acceptable, only double quote acceptable:{ex}");
            }
            // Then actually parse with Json.NET
             JToken.Parse(json);
             return true;
        }

        static MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }



        //public static async Task<bool> IsCheckMultipleErrors(string jsonString)
        //{
        //    //JsonSchema schemaFromFile = null;
        //    try
        //    {

        //        //var settings = new JsonSerializerSettings { Error = (se, ev) => { ev.ErrorContext.Handled = true; } };
        //        //dynamic d = JsonConvert.DeserializeObject(jsonString, settings);

        //        //dynamic d = JsonConvert.DeserializeObject(jsonString);
        //        //schemaFromFile = JsonSchema.FromSampleJson(jsonString);
        //        //var classGenerator = new CSharpGenerator(schemaFromFile, new CSharpGeneratorSettings
        //        //{
        //        //    ClassStyle = CSharpClassStyle.Poco,
        //        //});
        //        //var codeFile = classGenerator.GenerateFile();

        //        //var schema = JsonSchema.FromType<typeof()>();
        //        //var schemaData = schema.ToJson();
        //        // var errors = schema.Validate(jsonString);
        //        //loading schema from a file
        //        // schemaFromFile = JsonSchema.FromSampleJson(jsonString);
        //        //var errors = schemaFromFile.ToJson();
        //        //Validating the json file

        //    }
        //    catch (Exception e)
        //    {
        //        //var errors = schemaFromFile.ToJson();
        //    }

        //    return true;
        //}




    }
}
