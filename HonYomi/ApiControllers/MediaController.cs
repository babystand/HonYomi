using System;
using System.Linq;
using System.Threading.Tasks;
using DataLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HonYomi.ApiControllers
{
    [ApiController]
    public class MediaController : Controller
    {
        [HttpGet]
        [Authorize]
        [Route("/api/getfile/{id}")]
        //accepts byte range headers
        public async Task<FileStreamResult> GetAudioFile(Guid id)
        {
            string path, mimeType;
            using (var db = new HonyomiContext())
            {
                var file = await db.Files.FirstAsync(x => x.IndexedFileId == id);
                path = file.FilePath;
                mimeType = file.MimeType;
            }
            return File(System.IO.File.OpenRead(path), mimeType, true);

        }

        [HttpGet]
        [Authorize]
        [Route("/api/track/progress/get/{userId}/{trackId}")]
        public async Task<IActionResult> GetTrackProgress(string userId, Guid trackId)
        {
            try
            {
                using (var db = new HonyomiContext())
                {
                    return Json(await db.GetUserFileProgress(userId, trackId));
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/api/track/progress/set/{userId}/{trackId}/{seconds}")]
        public async Task<IActionResult> SetTrackProgress(string userId, Guid trackId, uint seconds)
        {
            try
            {
                using (var db = new HonyomiContext())
                {
                    FileProgress prog =
                       await db.FileProgresses.SingleOrDefaultAsync(x => x.UserId == userId && x.FileId == trackId);
                    if (prog == null)
                    {
                        await db.FileProgresses.AddAsync(
                            new FileProgress {FileId = trackId, UserId = userId, Progress = seconds});
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        prog.Progress = seconds;
                       await  db.SaveChangesAsync();
                    }

                    return Ok();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("/api/getlongfile")]
        public FileStreamResult GetLongFile()
        {
            const string path = "../HonYomi.Tests/long.mp3";
            return File(System.IO.File.OpenRead(path), "audio/mpeg", true);
        }
    }
}