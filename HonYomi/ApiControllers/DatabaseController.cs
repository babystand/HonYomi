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
        public DatabaseController()
        {
    
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("/api/db/clean")]
        public IActionResult RemoveMissing()
        {
            try
            {
     
                    DataAccess.RemoveMissing();
                    return Ok();
             
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("/api/db/scan")]
        public  IActionResult ScanNow()
        {
            try
            {
                DirectoryScanner.ScanWatchDirectories();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("/api/books/list")]
        public IActionResult GetBooksForUser()
        {
            Console.WriteLine(User.Identity.Name);
            try
            {
                var result =  DataAccess.GetUserBooks(User.Identity.Name);
                    return Json(result);
               
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
                [HttpGet]
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
                [Route("/api/books/list/fake")]
                public IActionResult GetBooksForUserFake()
                {
                    Console.WriteLine(User.Identity.Name);
                    try
                    {
                            var bookGuid = Guid.NewGuid();
                            var file1id = Guid.NewGuid();
                            var file2id = Guid.NewGuid();
                            var book = new BookWithProgress(){Guid = bookGuid, CurrentTrackGuid = file1id, FileProgresses = new []{new FileWithProgress(){BookGuid = bookGuid, BookTitle = "Book Title", Guid = file1id,ProgressSeconds = 5, Title = "track 1"}, new FileWithProgress(){BookGuid = bookGuid, BookTitle = "Book Title", Guid = file2id,ProgressSeconds = 66, Title = "track 2"}}};
                            return Json(new[]{book});
                      
                    }
                    catch (Exception)
                    {
                        return BadRequest();
                    }
                }

        [HttpGet, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme), Route("/api/db/config")]
        public  IActionResult GetConfig()
        {
            try
            {
                return Json( DataAccess.GetConfigClient());
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme), Route("/api/db/config")]
        public  IActionResult SetConfig([FromBody] ConfigClientModel model)
        {
            try
            {
                using (var db = new HonyomiContext())
                {
                    var config =  db.Configs.Include(x => x.WatchDirectories).First();
                    config.ScanInterval    = model.ScanInterval;
                    config.ServerPort      = model.ServerPort;
                    config.WatchForChanges = model.WatchForChanges;
                    //todo: be smarter about upserting
                    foreach (WatchDirClientModel dir in model.WatchDirectories)
                    {
                        if (config.WatchDirectories.All(x => x.Path != dir.Path))
                        {
                            db.WatchDirectories.Add(new WatchDirectory() {ConfigId = config.HonyomiConfigId, Path = dir.Path});
                        }
                    }

                    db.SaveChanges();
                }
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme), Route("/api/db/directories/add")]
        public IActionResult AddWatchDirectory([FromBody] string[] paths)
        {
            try
            {
                using (var db = new HonyomiContext())
                {
                    foreach (string path in paths)
                    {
                        db.WatchDirectories.Add(new WatchDirectory { Path = path, Config =  DataAccess.GetConfig() });
                    }

                    db.SaveChanges();
                    return Json( DataAccess.GetConfigClient());
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }

        }
        [HttpPost, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme), Route("/api/db/directories/remove")]
        public IActionResult RemoveWatchDirectory([FromBody] string[] paths)
        {
            try
            {
                using (var db = new HonyomiContext())
                {
                    foreach (string path in paths)
                    {
                        var wdRecord =  db.WatchDirectories.FirstOrDefault(x => x.Path == path);
                        if (wdRecord == null)
                        {
                            continue;
                        }
                        db.WatchDirectories.Remove(wdRecord);
                    }

                    db.SaveChanges();
                    return Json( DataAccess.GetConfigClient());
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }

        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("/api/books/get/{bookId}")]
        public  IActionResult GetBookForUser(Guid bookId)
        {
            try
            {
              
                    return Json( DataAccess.GetUserBookProgress(User.Identity.Name, bookId));
                
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}