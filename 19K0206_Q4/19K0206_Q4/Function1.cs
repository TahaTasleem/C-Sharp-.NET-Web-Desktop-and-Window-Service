using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace _19K0206_Q4
{
    public static class Function1
    {
        [FunctionName("myAzureFunction")]
        [FixedDelayRetry(5, "00:00:10")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "myAzureFunction/{name}")] HttpRequest req,string name
            ,ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string rootPath = @"C:\Users\mtaha\source\repos\k190206_Q2\bin\Debug\";
            string[] dirs = Directory.GetDirectories(rootPath, "*", SearchOption.AllDirectories);

            List<string> name11 = new List<string>();
            List<string> name2 = new List<string>();
            List<string> name3 = new List<string>();
            foreach (string dir in dirs)
            {
                name11.Add(dir.Substring(50));

            }
            string b;
            foreach (string a in name11)
            {
                b = a.Split(Path.DirectorySeparatorChar).Last();
                name3.Add(b);
                name2.Add(b.Replace(" ", ""));
            }
            int i = 0;string msg,msg2;
            foreach (string s1 in name2)
            {

                if (name == s1)
                {
                    break;
                }
                else
                {
                    i++;
                    msg2 = "Wrong Name";
                }
            }
            msg2 = "Wrong Name";

            string p1 = name11[i];
            string filepath = rootPath + " "+ p1 + "\\" + name3[i] + "." + ".json";
            var jsonData = System.IO.File.ReadAllText(filepath);
            msg = jsonData;

            string responseMessage = string.IsNullOrEmpty(name)
                ? msg2 : msg;

            return new OkObjectResult(responseMessage);
        }
    }
}
