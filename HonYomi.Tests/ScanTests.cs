using System.Linq;
using DataLib;
using HonYomi.Core;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Tests
{
    public class ScanTests
    {

        [Test]
        public void ScansBooksBasic()
        {
            string rootPath = "../../../scanTestDir";
            var books = DirectoryScanner.ScanDirectory(rootPath).ToArray();
            Assert.AreEqual(3, books.Length);
            Assert.IsTrue(books.Any(x => x.Name == "A1" && x.Files.Count == 2));
        }

        [Test]
        public void ScansBooksBasicAndInserts()
        {
            string rootPath = "../../../scanTestDir";
            var    books    = DirectoryScanner.ScanDirectory(rootPath).ToArray();
            using (var db = new HonyomiContext())
            {
                db.InsertNewBooks(books);
                Assert.AreEqual(3, db.Books.Count());
                Assert.IsTrue(db.Books.Include(x => x.Files).Any(x => x.Title == "A1" && x.Files.Count == 2));
            }
        }
    }
}