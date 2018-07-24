using System;
using System.Threading.Tasks;
using DataLib;
using HonYomi.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using HonYomi.Exposed;
using Microsoft.EntityFrameworkCore;

namespace HonYomi.ApiControllers
{
    [ApiController]
    public class DatabaseController : Controller
    {
        private HonyomiContext db;
        public DatabaseController(HonyomiContext dbcontext)
        {
            db = dbcontext;
        }

        [HttpGet]
        [Authorize]
        [Route("/api/db/clean")]
        public IActionResult RemoveMissing()
        {
            try
            {
                using (var db = new HonyomiContext())
                {
                    db.RemoveMissing();
                    return Ok();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/api/db/scan")]
        public async Task<IActionResult> ScanNow()
        {
            try
            {
                await DirectoryScanner.ScanWatchDirectories();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        [HttpGet]
        [Authorize]
        [Route("/api/books/list")]
        public async Task<IActionResult> GetBooksForUser()
        {
            Console.WriteLine(User.Identity.Name);
            try
            {
                using (var db = new HonyomiContext())
                {
                    return Json(await db.GetUserBooks(User.Identity.Name));
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
                [HttpGet]
                [Authorize]
                [Route("/api/books/list/fake")]
                public async Task<IActionResult> GetBooksForUserFake()
                {
                    Console.WriteLine(User.Identity.Name);
                    try
                    {
                        using (var db = new HonyomiContext())
                        {
                            var bookGuid = Guid.NewGuid();
                            var file1id = Guid.NewGuid();
                            var file2id = Guid.NewGuid();
                            var book = new BookWithProgress(){Guid = bookGuid, CurrentTrackGuid = file1id, FileProgresses = new []{new FileWithProgress(){BookGuid = bookGuid, BookTitle = "Book Title", Guid = file1id,ProgressSeconds = 5, Title = "track 1"}, new FileWithProgress(){BookGuid = bookGuid, BookTitle = "Book Title", Guid = file2id,ProgressSeconds = 66, Title = "track 2"}}};
                            return Json(new[]{book});
                        }
                    }
                    catch (Exception)
                    {
                        return BadRequest();
                    }
                }

        [HttpPost, Authorize, Route("/api/db/directories/add")]
        public async Task<IActionResult> AddWatchDirectory([FromBody] string[] paths)
        {
            try
            {
                foreach (string path in paths)
                {
                    await db.WatchDirectories.AddAsync(new WatchDirectory { Path = path, Config = await db.GetConfig() });
                }

                await db.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }

        }
        [HttpPost, Authorize, Route("/api/db/directories/remove")]
        public async Task<IActionResult> RemoveWatchDirectory([FromBody] string[] paths)
        {
            try
            {
                foreach (string path in paths)
                {
                    var wdRecord = await db.WatchDirectories.FirstOrDefaultAsync(x => x.Path == path);
                    if (wdRecord == null)
                    {
                        continue;
                    }
                    db.WatchDirectories.Remove(wdRecord);
                }

                await db.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }

        }

        [HttpGet]
        [Authorize]
        [Route("/api/books/get/{bookId}")]
        public async Task<IActionResult> GetBookForUser(Guid bookId)
        {
            try
            {
                using (var db = new HonyomiContext())
                {
                    return Json(await db.GetUserBookProgress(User.Identity.Name, bookId));
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}