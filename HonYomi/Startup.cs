using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataLib;
using Hangfire;
using Hangfire.MemoryStorage;
using HonYomi.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace HonYomi
{
    public class Startup
    {
        public static int port = 5000;
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddHangfire(x => x.UseMemoryStorage());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseStaticFiles();
            app.UseHangfireDashboard();
            app.UseHangfireServer();
            HonyomiConfig config;
            using (var db = new HonyomiContext())
            {
                config = db.Configs.First();
                port = config.ServerPort;
            }
            RecurringJob.AddOrUpdate("scan", () => DirectoryScanner.ScanWatchDirectories(), Cron.MinuteInterval(config.ScanInterval));

        }
    }
}