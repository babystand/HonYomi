using System.Linq;
using DataLib;
using HonYomi.Core;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace HonYomi.Tests
{
    public class ScanTests
    {
        private const string rootPath = "../../../scanTestDir";

        [Test]
        public void ScansBooksBasic()
        {
            var books = DirectoryScanner.ScanDirectory(rootPath).ToArray();
            Assert.AreEqual(3, books.Length);
            Assert.IsTrue(books.Any(x => x.Name == "A1" && x.Files.Count == 2));
        }

        [Test]
        public void ScansBooksBasicAndInserts()
        {
            var books = DirectoryScanner.ScanDirectory(rootPath).ToArray();
            HonyomiContext.DeleteDatabase();
            using (var db = new HonyomiContext())
            {
                db.Database.EnsureCreated();
                DataAccess.InsertNewBooks(books);
                Assert.AreEqual(3, db.Books.Count());
                Assert.IsTrue(db.Books.Include(x => x.Files).Any(x => x.Title == "A1" && x.Files.Count == 2));
            }
        }

        [Test]
        public void SearchesBooks()
        {
            void ShouldSearch(string input, BookInfo expected)
            {
                BookInfo result = BookSearch.Search(input);
                Assert.AreEqual(expected.Title, result.Title );
                Assert.AreEqual(expected.Author, result.Author );
                Assert.AreEqual(expected.ISBN, result.ISBN );
            }
            ShouldSearch("Brandon Sanderson - Mistborn 02 - The Well of Ascension  pt 1", new BookInfo(){Title = "The Well of Ascension (Mistborn, Book 2)", Author = "Brandon Sanderson", ISBN = "9780765316882"});
        }
        
        
    }
}