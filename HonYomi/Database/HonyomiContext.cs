using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HonYomi.Core;
using HonYomi.Exposed;
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

            if (!File.Exists(RuntimeConstants.DatabaseLocation))
            {
                Directory.CreateDirectory(RuntimeConstants.DataDir);
                File.Create(RuntimeConstants.DatabaseLocation);
            }
            optionsBuilder.UseSqlite($"Data Source={RuntimeConstants.DatabaseLocation}");

        }

        public static void DeleteDatabase()
        {
            if(File.Exists(RuntimeConstants.DatabaseLocation))
                File.Delete(RuntimeConstants.DatabaseLocation);
        }
        

        public HonyomiContext()
        {
            //true if database had to be created
            if (Database.EnsureCreated())
            {
                CreateDefaults();
            }
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
                        FilePath   = x.Path,
                        MimeType = x.MimeType
                    }).ToList();
                    Books.Add(new IndexedBook() {DirectoryPath = book.Path, Files = files, Title = book.Name});
                }
            }

            SaveChanges();
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

        public FileWithProgress GetUserFileProgress(Guid userId, Guid fileId)
        {
            IndexedFile file = Files.Include(x => x.Book).Single(x => x.IndexedFileId == fileId);
            FileWithProgress result = new FileWithProgress
            {
                Guid      = fileId,
                Title     = file.Title,
                BookGuid  = file.BookId,
                BookTitle = file.Book.Title
            };
            FileProgress fProg = FileProgresses.SingleOrDefault(x => x.FileId == fileId && x.UserId == userId);
            if (fProg == null)
                result.ProgressSeconds = 0;
            else
                result.ProgressSeconds = fProg.Progress;
            return result;
        }

        public BookWithProgress GetUserBookProgress(Guid userId, Guid bookId)
        {
            IndexedBook book = Books.Include(x => x.Files).Single(x => x.IndexedBookId == bookId);
            
            BookProgress bookp = 
                BookProgresses.SingleOrDefault(x => x.BookId == bookId && x.UserId == userId);
            BookWithProgress result = new BookWithProgress
            {
                FileProgresses   = book.Files.Select(x => GetUserFileProgress(userId, x.IndexedFileId)).ToArray(),
                Guid             = book.IndexedBookId,
                CurrentTrackGuid = bookp?.FileId ?? book.Files.First().IndexedFileId,
                Title            = book.Title
            };
            return result;
        }

        public BookWithProgress[] GetUserBooks(Guid userId)
        {
            return Books.Select(x => GetUserBookProgress(userId, x.IndexedBookId)).ToArray();
        }
    }
}