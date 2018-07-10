using System;
using System.Linq;
using DataLib;
using Microsoft.AspNetCore.Mvc;

namespace HonYomi.ApiControllers
{
    public class MediaController : Controller
    {
        public FileStreamResult GetAudioFile(Guid id)
        {
            string path;
            using (var db = new HonyomiContext())
            {
                path = db.Files.First(x => x.IndexedFileId == id).FilePath;
            }
            return File(System.IO.File.OpenRead(path), "audio/mp3", true);

        }
    }
}