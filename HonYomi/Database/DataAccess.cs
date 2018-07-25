using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HonYomi.Core;
using HonYomi.Exposed;
using Microsoft.EntityFrameworkCore;

namespace DataLib
{
    public static class DataAccess
    {
          public static HonyomiConfig GetConfig(){
              using (var db = new HonyomiContext())
              {
                  return db.Configs.First();
              }
          }

        public static ConfigClientModel GetConfigClient()
        {
            using (var db = new HonyomiContext())
            {
                var config =  db.Configs.Include(x => x.WatchDirectories).First();
                return new ConfigClientModel(config.WatchForChanges, config.ScanInterval, config.ServerPort, config.WatchDirectories.Select(x => new WatchDirClientModel(x.Path, x.WatchDirectoryId)).ToArray());
            }
        }

        internal static void InsertNewBooks(IEnumerable<ScannedBook> books)
        {
            using (var db = new HonyomiContext())
            {
                foreach (ScannedBook book in books)
                {
                    if (!db.Books.Any(x => x.DirectoryPath == book.Path))
                    {
                        var files = book.Files.Select(x => new IndexedFile()
                                                           {
                                                               TrackIndex = x.Index,
                                                               Filename   = x.Name,
                                                               Title      = x.Name,
                                                               FilePath   = x.Path,
                                                               MimeType   = x.MimeType
                                                           }).ToList();
                        db.Books.Add(new IndexedBook() {DirectoryPath = book.Path, Files = files, Title = book.Name});
                    }
                }

                db.SaveChanges();
            }
        }

        public static void RemoveMissing()
        {
            using (var db = new HonyomiContext())
            {
                List<Guid> removedFileIds = new List<Guid>();
                List<Guid> removedBookIds = new List<Guid>();
                foreach (IndexedFile file in db.Files)
                {
                    if (!File.Exists(file.FilePath))
                    {
                        db.Files.Remove(file);
                        removedFileIds.Add(file.IndexedFileId);
                    }
                }

                foreach (IndexedBook book in db.Books)
                {
                    if (!book.Files.Any())
                    {
                        db.Books.Remove(book);
                        removedBookIds.Add(book.IndexedBookId);
                    }
                }

                foreach (Guid fileId in removedFileIds)
                {
                    var referencedFileProgresses = db.FileProgresses.Where(x => x.FileId == fileId);
                    db.FileProgresses.RemoveRange(referencedFileProgresses);
                }

                foreach (Guid bookId in removedBookIds)
                {
                    var referencedBookProgresses = db.BookProgresses.Where(x => x.BookId == bookId);
                    db.BookProgresses.RemoveRange(referencedBookProgresses);
                }

                db.SaveChanges();

                
            }
        }

        public static FileWithProgress GetUserFileProgress(string userId, Guid fileId)
        {
            using (var db = new HonyomiContext())
            {
                IndexedFile file =  db.Files.Include(x => x.Book).SingleOrDefault(x => x.IndexedFileId == fileId);
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
                FileProgress fProg =  db.FileProgresses.SingleOrDefault(x => x.FileId == fileId && x.UserId == userId);
                if (fProg == null)
                    result.ProgressSeconds = 0;
                else
                    result.ProgressSeconds = fProg.Progress;
                return result;
            }
        }

        public static BookWithProgress GetUserBookProgress(string userId, Guid bookId)
        {
            using (var db = new HonyomiContext())
            {
                IndexedBook book = db.Books.Include(x => x.Files).Single(x => x.IndexedBookId == bookId);
                if (book == null)
                {
                    return null;
                }
                BookProgress bookp = 
                    db.BookProgresses.SingleOrDefault(x => x.BookId == bookId && x.UserId == userId);
                BookWithProgress result = new BookWithProgress
                                          {
                                              FileProgresses   =  book.Files.Select( x =>  GetUserFileProgress(userId, x.IndexedFileId)).ToArray(),
                                              Guid             = book.IndexedBookId,
                                              CurrentTrackGuid = bookp?.FileId ?? book.Files.First().IndexedFileId,
                                              Title            = book.Title
                                          };
                return result;
            }
        }

        public static void AdvanceBookProgress(string userId, Guid bookId)
        {
            using (var db = new HonyomiContext())
            {
                BookProgress progress =  db.BookProgresses.Include(x => x.Book).ThenInclude(x => x.Files).Include(x => x.File)
                                                       .SingleOrDefault(x => x.BookId == bookId && x.UserId == userId);
                //Initialize if missing
                if (progress == null)
                {
                    IndexedBook book =  db.Books.Include(x => x.Files).SingleOrDefault(x => x.IndexedBookId == bookId);
                    if (book == null)
                    {
                        //bail when we can't create a bookprogress for a book that doesn't exist
                        return;
                    }

                    db.BookProgresses.Add(new BookProgress()
                                       {
                                           Book   = book,
                                           File   = book.Files.OrderBy(x => x.TrackIndex).First(),
                                           UserId = userId
                                       });
                    db.SaveChanges();
                    return;
                }
                var         nextIndex = progress.File?.TrackIndex + 1 ?? 0;
                IndexedFile nextFile  =  progress.Book.Files.SingleOrDefault(x => x.TrackIndex == nextIndex);
                if (nextFile == null)
                {
                    //if there is no next file, bail
                    return;
                }

                progress.File = nextFile;
                db.SaveChanges();
                
            }
        }

        public static BookWithProgress[] GetUserBooks(string userId)
        {
            using (var db = new HonyomiContext())
            {

                return db.Books.Select( x =>   GetUserBookProgress(userId, x.IndexedBookId)).ToArray();
            }
        }
    }
}