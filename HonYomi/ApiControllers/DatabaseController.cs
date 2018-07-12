using System;
using DataLib;
using HonYomi.Core;
using Microsoft.AspNetCore.Mvc;

namespace HonYomi.ApiControllers
{
    [ApiController]
    public class DatabaseController : Controller
    {
        [HttpGet]
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
        [Route("/api/db/scan")]
        public IActionResult ScanNow()
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
        [Route("/api/db/books/{userId}")]
        public IActionResult GetBooksForUser(Guid userId)
        {
            try
            {
                using (var db = new HonyomiContext())
                {
                    return Json(db.GetUserBooks(userId));
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}