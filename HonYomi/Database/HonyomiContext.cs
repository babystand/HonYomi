using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HonYomi.Core;
using Microsoft.EntityFrameworkCore;

namespace DataLib
{
    internal sealed class HonyomiContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<IndexedBook> Books { get; set; }
        public DbSet<IndexedFile> Files { get; set; }
        public DbSet<BookProgress> BookProgresses { get; set; }
        public DbSet<FileProgress> FileProgresses { get; set; }
        public DbSet<HonyomiConfig> Configs { get; set; }
        public DbSet<WatchDirectory> WatchDirectories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config",
                                        "onyomi");
            var dbPath = Path.Combine(dataPath, "onyomi.db");
            if (!File.Exists(dbPath))
            {
                Directory.CreateDirectory(dataPath);
                File.Create(dbPath);
            }
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

        }

        public static void DeleteDatabase()
        {
            var dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config",
                                        "onyomi");
            var dbPath = Path.Combine(dataPath, "onyomi.db");
            if(File.Exists(dbPath))
                File.Delete(dbPath);
        }
        

        public HonyomiContext()
        {
            Database.EnsureCreated();
        }

        public void CreateDefaults()
        {
            Users.Add(new User {Username = "admin", HashedPass = "9BC7AA55F08FDAD935C3F8362D3F48BCF70EB280", HashSalt = "salt", IsAdmin = true});
            Configs.Add(new HonyomiConfig{ScanInterval = 0, ServerPort = 5367, WatchForChanges = false});
            SaveChanges();
        }

        public void InsertNewBooks(IEnumerable<ScannedBook> books)
        {
            foreach (ScannedBook book in books)
            {
                if (!Books.Any(x => x.DirectoryPath == book.Path))
                {
                    var files = book.Files.Select(x => new IndexedFile()
                    {
                        TrackIndex = x.Index,
                        Filename   = x.Name,
                        Title      = x.Name,
                        FilePath   = x.Path
                    }).ToList();
                    Books.Add(new IndexedBook() {DirectoryPath = book.Path, Files = files, Title = book.Name});
                }
            }

            SaveChanges();
        }
    }
}