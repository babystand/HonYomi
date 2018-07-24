﻿using System;
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            optionsBuilder.UseSqlite($"Data Source={RuntimeConstants.DatabaseLocation}");

        }
        

        public static void DeleteDatabase()
        {
            if(File.Exists(RuntimeConstants.DatabaseLocation))
                File.Delete(RuntimeConstants.DatabaseLocation);
        }




        public async Task CreateDefaults(UserManager<IdentityUser> userManager)
        {
            var user = new IdentityUser("admin");
            await userManager.CreateAsync(user, "adminPassword");
            await Configs.AddAsync(new HonyomiConfig{ScanInterval = 59, ServerPort = 5367, WatchForChanges = false});
            await SaveChangesAsync();
        }
        public async Task<HonyomiConfig> GetConfig(){
            return await Configs.FirstAsync();
        }

        public async Task<ConfigClientModel> GetConfigClient()
        {
            var config = await Configs.Include(x => x.WatchDirectories).FirstAsync();
            return new ConfigClientModel(config.WatchForChanges, config.ScanInterval, config.ServerPort, config.WatchDirectories.Select(x => new WatchDirClientModel(x.Path, x.WatchDirectoryId)).ToArray());
        }

        internal async Task InsertNewBooks(IEnumerable<ScannedBook> books)
        {
            foreach (ScannedBook book in books)
            {
                if (!await Books.AnyAsync(x => x.DirectoryPath == book.Path))
                {
                    var files = book.Files.Select(x => new IndexedFile()
                    {
                        TrackIndex = x.Index,
                        Filename   = x.Name,
                        Title      = x.Name,
                        FilePath   = x.Path,
                        MimeType = x.MimeType
                    }).ToList();
                    await Books.AddAsync(new IndexedBook() {DirectoryPath = book.Path, Files = files, Title = book.Name});
                }
            }

            await SaveChangesAsync();
        }

        public void RemoveMissing()
        {
            List<Guid> removedFileIds = new List<Guid>();
            List<Guid> removedBookIds = new List<Guid>();
            foreach (IndexedFile file in Files)
            {
                if (!File.Exists(file.FilePath))
                {
                    Files.Remove(file);
                    removedFileIds.Add(file.IndexedFileId);
                }
            }

            foreach (IndexedBook book in Books)
            {
                if (!book.Files.Any())
                {
                    Books.Remove(book);
                    removedBookIds.Add(book.IndexedBookId);
                }
            }

            foreach (Guid fileId in removedFileIds)
            {
                var referencedFileProgresses = FileProgresses.Where(x => x.FileId == fileId);
                FileProgresses.RemoveRange(referencedFileProgresses);
            }

            foreach (Guid bookId in removedBookIds)
            {
                var referencedBookProgresses = BookProgresses.Where(x => x.BookId == bookId);
                BookProgresses.RemoveRange(referencedBookProgresses);
            }

            SaveChanges();


        }

        public async Task<FileWithProgress> GetUserFileProgress(string userId, Guid fileId)
        {
            IndexedFile file = await Files.Include(x => x.Book).SingleOrDefaultAsync(x => x.IndexedFileId == fileId);
            if (file == null)
            {
                //bail when such a file does not exist
                return null;
            }
            FileWithProgress result = new FileWithProgress
            {
                Guid      = fileId,
                Title     = file.Title,
                BookGuid  = file.BookId,
                BookTitle = file.Book.Title
            };
            FileProgress fProg = await FileProgresses.SingleOrDefaultAsync(x => x.FileId == fileId && x.UserId == userId);
            if (fProg == null)
                result.ProgressSeconds = 0;
            else
                result.ProgressSeconds = fProg.Progress;
            return result;
        }

        public async Task<BookWithProgress> GetUserBookProgress(string userId, Guid bookId)
        {
            IndexedBook book = await Books.Include(x => x.Files).SingleAsync(x => x.IndexedBookId == bookId);
            if (book == null)
            {
                return null;
            }
            BookProgress bookp = 
                await BookProgresses.SingleOrDefaultAsync(x => x.BookId == bookId && x.UserId == userId);
            BookWithProgress result = new BookWithProgress
            {
                FileProgresses   = await Task.WhenAll(book.Files.Select(async x => await GetUserFileProgress(userId, x.IndexedFileId))),
                Guid             = book.IndexedBookId,
                CurrentTrackGuid = bookp?.FileId ?? book.Files.First().IndexedFileId,
                Title            = book.Title
            };
            return result;
        }

        public async Task AdvanceBookProgress(string userId, Guid bookId)
        {
            BookProgress progress = await BookProgresses.Include(x => x.Book).ThenInclude(x => x.Files).Include(x => x.File)
                                                  .SingleOrDefaultAsync(x => x.BookId == bookId && x.UserId == userId);
            //Initialize if missing
            if (progress == null)
            {
                IndexedBook book = await Books.Include(x => x.Files).SingleOrDefaultAsync(x => x.IndexedBookId == bookId);
                if (book == null)
                {
                    //bail when we can't create a bookprogress for a book that doesn't exist
                    return;
                }

                await BookProgresses.AddAsync(new BookProgress()
                                   {
                                       Book   = book,
                                       File   = book.Files.OrderBy(x => x.TrackIndex).First(),
                                       UserId = userId
                                   });
                await SaveChangesAsync();
                return;
            }
            var nextIndex = progress.File?.TrackIndex + 1 ?? 0;
            IndexedFile nextFile =  progress.Book.Files.SingleOrDefault(x => x.TrackIndex == nextIndex);
            if (nextFile == null)
            {
                //if there is no next file, bail
                return;
            }

            progress.File = nextFile;
            await SaveChangesAsync();

        }

        public async Task<BookWithProgress[]> GetUserBooks(string userId)
        {
            return  await Task.WhenAll(Books.Select( x =>   GetUserBookProgress(userId, x.IndexedBookId)));
        }
    }
}