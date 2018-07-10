using System;
using System.Linq;
using DataLib;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HonYomi.ApiControllers
{
    [ApiController]
    public class MediaController : Controller
    {
        [HttpGet]
        [Route("/api/getfile/{id}")]
        public FileStreamResult GetAudioFile(Guid id)
        {
            string path;
            using (var db = new HonyomiContext())
            {
                path = db.Files.First(x => x.IndexedFileId == id).FilePath;
            }
            return File(System.IO.File.OpenRead(path), "audio/mpeg", true);

        }

        [HttpGet]
        [Route("/api/getlongfile")]
        public FileStreamResult GetLongFile()
        {
            string path = "../HonYomi.Tests/long.mp3";
            return File(System.IO.File.OpenRead(path), "audio/mpeg", true);
        }
    }
}