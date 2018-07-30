using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HonYomi.ApiControllers;
using HonYomi.Core;
using HonYomi.Exposed;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataLib
{
    public sealed class HonyomiContext : IdentityDbContext
    {
        public DbSet<IndexedBook> Books { get; set; }
        public DbSet<IndexedFile> Files { get; set; }
        public DbSet<BookProgress> BookProgresses { get; set; }
        public DbSet<FileProgress> FileProgresses { get; set; }
        public DbSet<HonyomiConfig> Configs { get; set; }
        public DbSet<WatchDirectory> WatchDirectories { get; set; }
        public DbSet<TempMediaLocation> TempMediaLocations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            optionsBuilder.UseSqlite($"Data Source={RuntimeConstants.DatabaseLocation}");

        }
        

        public static void DeleteDatabase()
        {
            if(File.Exists(RuntimeConstants.DatabaseLocation))
                File.Delete(RuntimeConstants.DatabaseLocation);
        }




        public void CreateDefaults(UserManager<IdentityUser> userManager)
        {
            var user = new IdentityUser("admin");
            userManager.CreateAsync(user, "adminPassword").Wait();
            Configs.Add(new HonyomiConfig{ScanInterval = 59, ServerPort = 5367, WatchForChanges = false});
            SaveChanges();
        }
      
    }
}