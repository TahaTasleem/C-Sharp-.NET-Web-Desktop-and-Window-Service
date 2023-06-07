using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Collections;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace _19K0206_Q3
{
    public class ScriptData
    {
        public DateTime lastUpdatedOn { get; set; }
        public object[] allPrices { get; set; }
    }
    public class Pricedate
    {
        public DateTime date { get; set; }
        public object price { get; set; }
        public Pricedate(DateTime date, object price)
        {
            this.date = date;
            this.price = price;
        }
    }
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([TimerTrigger("* /20 * * * *")]TimerInfo myTimer, ILogger log)
        {
            json();
        }
        static void json()
        {
            List<string> name = new List<string>();
            //Rootpath is defined here, which is used in every path
            string rootPath = "C:\\Users\\mtaha\\source\\repos\\k190206_Q2\\bin\\Debug\\";
            string[] dirs = Directory.GetDirectories(rootPath, "*", SearchOption.TopDirectoryOnly);
            foreach (string dir in dirs)
            {
                name.Add(dir.Substring(dir.LastIndexOf("\\") + 1));
            }
            foreach (string file1 in name)
            {
                Console.WriteLine(file1);
                string doc_del = rootPath + file1 + "\\" + file1 + ".xml";
                var doc = XDocument.Load(rootPath + file1 + "\\" + file1 + ".xml");
                var nodes = from node in doc.Descendants("Scripts") select node;
                dynamic obj = new ExpandoObject();
                var arr = new ArrayList();
                var arr2 = new ArrayList();

                foreach (var child in nodes.Descendants())
                {
                    (obj as IDictionary<String, Object>)[child.Name.ToString()] = child.Value.Trim();
                    if (child.Name.ToString() == "script")
                    {
                        arr.Add(obj.script);
                    }
                    if (child.Name.ToString() == "Price")
                    {
                        arr2.Add(obj.Price.ToString());
                    }

                }
                var i = 0;
                DateTime now = DateTime.Now;
                foreach (string filename in arr)
                {
                    string directory = rootPath + file1 + "\\ " + filename;
                    string file = directory + "\\ " + filename + ".json";

                    if (!Directory.Exists(directory))
                        System.IO.Directory.CreateDirectory(directory);

                    if (!File.Exists(file))
                    {
                        ScriptData data = new ScriptData()
                        {
                            lastUpdatedOn = now,
                            allPrices = new object[]
                               {
                               new Pricedate(now,arr2[i])
                               }
                        };
                        i++;
                        string jsonString = JsonConvert.SerializeObject(data);
                        System.IO.File.WriteAllText(file, jsonString);

                    }
                    else
                    {
                        var jsonData = System.IO.File.ReadAllText(file);
                        JObject datalist = JsonConvert.DeserializeObject<JObject>(jsonData);
                        JObject jobject = JObject.Parse(datalist.ToString());
                        jobject.Property("lastUpdatedOn").Value = now.ToString();
                        JArray item = (JArray)jobject["allPrices"];
                        JObject newItem = new JObject();
                        newItem["date"] = now;
                        newItem["price"] = arr2[i].ToString();
                        item.Add(newItem);
                        jsonData = JsonConvert.SerializeObject(jobject);
                        System.IO.File.WriteAllText(file, jsonData);
                    }
                }
                File.Delete(doc_del);
            }

        }
    }
}
