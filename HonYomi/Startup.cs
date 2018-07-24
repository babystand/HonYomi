using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLib;
using Hangfire;
using Hangfire.MemoryStorage;
using HonYomi.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

namespace HonYomi
{
    public class Startup
    {
        public static int port = 5000;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //delete db for debugging

            if (File.Exists(RuntimeConstants.DatabaseLocation))
                File.Delete(RuntimeConstants.DatabaseLocation);

            /***EF Core***/
            services.AddDbContext<HonyomiContext>();
            /***Identity***/
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
                                                             {
                                                                 options.Password.RequireNonAlphanumeric = false;
                                                                 options.Password.RequiredLength         = 8;
                                                                 options.Password.RequireUppercase       = true;
                                                                 options.Password.RequireLowercase       = true;
                                                                 options.Password.RequireDigit           = false;
                                                                 options.Password.RequiredUniqueChars    = 5;
                                                             }).AddEntityFrameworkStores<HonyomiContext>()
                    .AddDefaultTokenProviders();
            services.AddTransient<UserManager<IdentityUser>>();
            /***JWT config***/
            //clear defaults
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
                                       {
                                           options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                           options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
                                       }).AddJwtBearer(cfg =>
                                                       {
                                                           cfg.ClaimsIssuer         = RuntimeConstants.JwtIssuer;
                                                           cfg.RequireHttpsMetadata = false;
                                                           cfg.SaveToken            = true;

                                                           cfg.TokenValidationParameters =
                                                               new TokenValidationParameters()
                                                               {
                                                                   ValidIssuer   = RuntimeConstants.JwtIssuer,
                                                                   ValidAudience = RuntimeConstants.JwtIssuer,
                                                                   IssuerSigningKey =
                                                                       new
                                                                           SymmetricSecurityKey(Encoding
                                                                                                .UTF8
                                                                                                .GetBytes(RuntimeConstants
                                                                                                              .JwtKey)),
                                                                   ClockSkew = TimeSpan.FromMinutes(5)
                                                               };
                                                       });
            services.AddAuthorization();
            /***MVC***/
            services.AddMvc();
            /***Scheduled Background Tasks (Hangfire)***/
            services.AddHangfire(x => x.UseMemoryStorage());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder       app, IHostingEnvironment env, HonyomiContext dbContext,
                              UserManager<IdentityUser> uMan)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            /***MVC***/
            app.UseMvc();

            /***Authentication  ***/
            app.UseAuthentication();

            /***Configure static files at root address "ui" instead of "wwwroot/dist" ***/
            app.UseStaticFiles(new StaticFileOptions()
                               {
                                   FileProvider =
                                       new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),
                                                                             "wwwroot"))
                               });

            /***Add Hangfire for Background tasks***/
            app.UseHangfireDashboard();
            app.UseHangfireServer();

            /***Create DB and populate defaults if needed***/
            if (dbContext.Database.EnsureCreated())
            {
                dbContext.CreateDefaults(uMan).Wait();
            }

            HonyomiConfig config = dbContext.Configs.First();
            port = config.ServerPort;
            //debug scan path
            if (env.IsDevelopment())
            {
                dbContext.WatchDirectories.Add(new WatchDirectory()
                                               {
                                                   ConfigId = config.HonyomiConfigId,
                                                   Path = Path.Combine(Directory.GetCurrentDirectory(),
                                                                       "..", "HonYomi.Tests",
                                                                       "scanTestDir")
                                               });
                dbContext.SaveChanges();
            }

            DirectoryScanner.ScanWatchDirectories().Wait();
            RecurringJob.AddOrUpdate("scan", () => DirectoryScanner.ScanWatchDirectories(),
                                     Cron.MinuteInterval(config.ScanInterval));
        }
    }
}