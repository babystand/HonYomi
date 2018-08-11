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
        public static HonyomiConfig GetConfig()
        {
            using (var db = new HonyomiContext())
            {
                return db.Configs.First();
            }
        }

        public static ConfigClientModel GetConfigClient()
        {
            using (var db = new HonyomiContext())
            {
                var config = db.Configs.Include(x => x.WatchDirectories).First();
                return new ConfigClientModel(config.WatchForChanges, config.ScanInterval, config.ServerPort,
                                             config.WatchDirectories
                                                 .Select(x => new WatchDirClientModel(x.Path, x.WatchDirectoryId))
                                                 .ToArray());
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
                        var ibook = new IndexedBook() {DirectoryPath = book.Path, Title = book.Name};
                        var bookInfo = book.Files.Select(x => BookSearch.Search(x.Name)).FirstOrDefault(x => x != null);
                        if (bookInfo != null)
                        {
                            ibook.Author = bookInfo.Author;
                            ibook.ISBN = bookInfo.ISBN;
                            ibook.Title = bookInfo.Title;
                        }
                        var files = book.Files.Select(x => new IndexedFile()
                        {
                            TrackIndex = x.Index,
                            Filename   = x.Name,
                            Title      = x.Name,
                            FilePath   = x.Path,
                            MimeType   = x.MimeType,
                            Duration   = x.Duration.TotalSeconds
                        }).ToList();
                        ibook.Files = files;
                        db.Books.Add(ibook);
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
                IndexedFile file = db.Files.Include(x => x.Book).ThenInclude(x => x.Files)
                    .SingleOrDefault(x => x.IndexedFileId == fileId);
                if (file == null)
                {
                    //bail when such a file does not exist
                    return null;
                }

                FileWithProgress result = new FileWithProgress
                {
                    Guid       = fileId,
                    Title      = file.Title,
                    BookGuid   = file.BookId,
                    BookTitle  = file.Book.Title,
                    TrackIndex = file.TrackIndex,
                    MediaType  = file.MimeType,
                    Duration = file.Duration,
                    NextFile =
                        file.Book.Files.FirstOrDefault(x => x.TrackIndex == file.TrackIndex + 1)?.IndexedFileId ??
                        Guid.Empty
                };
                FileProgress fProg = db.FileProgresses.SingleOrDefault(x => x.FileId == fileId && x.UserId == userId);
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
                    FileProgresses =
                        book.Files.Select(x => GetUserFileProgress(userId, x.IndexedFileId))
                            .ToArray(),
                    Guid             = book.IndexedBookId,
                    CurrentTrackGuid = bookp?.FileId ?? book.Files.First().IndexedFileId,
                    Title            = book.Title,
                    Author = book.Author,
                    ISBN = book.ISBN
                };
                return result;
            }
        }

        /// <summary>
        /// Sets the current track for the parent book to this track
        /// Generates BookProgress if required
        /// </summary>
        public static void SetCurrentTrack(string userId, Guid trackId)
        {
            using (var db = new HonyomiContext())
            {
                IndexedBook book = db.Files.Include(x => x.Book).SingleOrDefault(x => x.IndexedFileId == trackId)?.Book;
                if (book == null)
                {
                    //can't update progress on a book the doesn't exist
                    return;
                }


                BookProgress prog =
                    db.BookProgresses.SingleOrDefault(x => x.BookId == book.IndexedBookId && x.UserId == userId);
                if (prog == null)
                {
                    db.BookProgresses.Add(new BookProgress()
                    {
                        BookId = book.IndexedBookId,
                        FileId = trackId,
                        UserId = userId
                    });
                }
                else
                    prog.FileId = trackId;

                db.SaveChanges();
            }
        }

        public static void SetTrackProgress(string userId, Guid trackId, double seconds)
        {
            using (var db = new HonyomiContext())
            {
                FileProgress prog = db.FileProgresses.SingleOrDefault(x => x.FileId == trackId && x.UserId == userId);
                if (prog == null)
                {
                    db.FileProgresses.Add(new FileProgress() {FileId = trackId, UserId = userId, Progress = seconds});
                }
                else
                {
                    prog.Progress = seconds;
                }

                db.SaveChanges();
            }
        }


        public static BookWithProgress[] GetUserBooks(string userId)
        {
            using (var db = new HonyomiContext())
            {
                return db.Books.Select(x => GetUserBookProgress(userId, x.IndexedBookId)).ToArray();
            }
        }
    }
}