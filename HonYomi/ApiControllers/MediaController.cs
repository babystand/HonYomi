using System;
using System.Linq;
using System.Threading.Tasks;
using DataLib;
using HonYomi.Exposed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("/api/tracks/reserve/{guid}")]
        public IActionResult ReserveAudioFile(Guid fileId)
        {
            try
            {
                TempMediaLocation tempMediaLocation =
                    DataAccess.CreateTempMediaLocation(fileId, DateTime.Now + TimeSpan.FromHours(12));
                return Ok(new FileReservation{Url = $"/api/tracks/stream/{tempMediaLocation.Id}", FileId = fileId, MimeType = tempMediaLocation.File.MimeType});
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        
        [HttpGet]
        [Route("/api/tracks/stream/{id}")]
        //accepts byte range headers
        public IActionResult GetAudioFile(Guid id)
        {
            string path, mimeType;
            using (var db = new HonyomiContext())
            {
                var file =  db.TempMediaLocations.Include(x => x.File).FirstOrDefault(x => x.Id == id);
                if (file == null || file.Expires < DateTime.Now)
                {
                    return NotFound();
                }
                path = file.File.FilePath;
                mimeType = file.File.MimeType;
            }
            return File(System.IO.File.OpenRead(path), mimeType, true);

        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("/api/tracks/progress/get/{trackId}")]
        public IActionResult GetTrackProgress(Guid trackId)
        {
            try
            {
        
                    return Json( DataAccess.GetUserFileProgress(User.Identity.Name, trackId));
                
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("/api/tracks/progress/set/{trackId}/{seconds}")]
        public IActionResult SetTrackProgress(Guid trackId, uint seconds)
        {
            try
            {
                using (var db = new HonyomiContext())
                {
                    FileProgress prog =
                        db.FileProgresses.SingleOrDefault(x => x.UserId == User.Identity.Name && x.FileId == trackId);
                    if (prog == null)
                    {
                         db.FileProgresses.AddAsync(
                            new FileProgress {FileId = trackId, UserId = User.Identity.Name, Progress = seconds});
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