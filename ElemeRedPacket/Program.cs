using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ElemeRedPacket
{
    public class Program
    {
        public static Data.AppConfig Config;
        public static void Main(string[] args)
        {
            StreamReader sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"), Encoding.Default);
            string config = sr.ReadToEnd();
            Config = JsonConvert.DeserializeObject<Data.AppConfig>(config);
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())//开启默认静态路径wwwroot
                .UseStartup<Startup>()
                //.UseUrls("http://*:9999")//修改默认监听端口
                .Build();
    }
}
