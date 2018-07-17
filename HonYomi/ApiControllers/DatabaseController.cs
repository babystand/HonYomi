using System;
using System.Threading.Tasks;
using DataLib;
using HonYomi.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HonYomi.ApiControllers
{
    [ApiController]
    public class DatabaseController : Controller
    {
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
            Console.WriteLine(  User.Identity.Name);
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
        [Route("/api/books/get/{userId}/{bookId}")]
        public async Task<IActionResult> GetBookForUser(Guid bookId, string userId)
        {
            try
            {
                using (var db = new HonyomiContext())
                {
                    return Json(await db.GetUserBookProgress(userId, bookId));
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}