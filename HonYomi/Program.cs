using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataLib;
using Hangfire;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HonYomi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //create missing db file
            if (!File.Exists(RuntimeConstants.DatabaseLocation))
            {
                Directory.CreateDirectory(RuntimeConstants.DataDir);
                File.Create(RuntimeConstants.DatabaseLocation).Close();
            }
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();  //todo: for dynamic config port, use //.UseUrls($"http://0.0.0.0:{Startup.port}/");
    }
}