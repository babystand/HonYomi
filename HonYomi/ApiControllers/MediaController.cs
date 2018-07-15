using System;
using System.Linq;
using DataLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HonYomi.ApiControllers
{
    [ApiController]
    public class MediaController : Controller
    {
        [HttpGet]
        [Authorize]
        [Route("/api/getfile/{id}")]
        public FileStreamResult GetAudioFile(Guid id)
        {
            string path, mimeType;
            using (var db = new HonyomiContext())
            {
                var file = db.Files.First(x => x.IndexedFileId == id);
                path = file.FilePath;
                mimeType = file.MimeType;
            }
            return File(System.IO.File.OpenRead(path), mimeType, true);

        }

        [HttpGet]
        [Authorize]
        [Route("/api/trackprogress/get/{userId}/{trackId}")]
        public IActionResult GetTrackProgress(string userId, Guid trackId)
        {
            try
            {
                using (var db = new HonyomiContext())
                {
                    return Json(db.GetUserFileProgress(userId, trackId));
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("/api/trackprogress/set/{userId}/{trackId}/{seconds}")]
        public IActionResult SetTrackProgress(string userId, Guid trackId, uint seconds)
        {
            try
            {
                using (var db = new HonyomiContext())
                {
                    FileProgress prog =
                        db.FileProgresses.SingleOrDefault(x => x.UserId == userId && x.FileId == trackId);
                    if (prog == null)
                    {
                        db.FileProgresses.Add(
                            new FileProgress {FileId = trackId, UserId = userId, Progress = seconds});
                        db.SaveChanges();
                    }
                    else
                    {
                        prog.Progress = seconds;
                        db.SaveChanges();
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