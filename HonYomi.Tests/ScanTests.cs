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
                DataAccess.InsertNewBooks(books);
                Assert.AreEqual(3, db.Books.Count());
                Assert.IsTrue(db.Books.Include(x => x.Files).Any(x => x.Title == "A1" && x.Files.Count == 2));
            }
        }
    }
}