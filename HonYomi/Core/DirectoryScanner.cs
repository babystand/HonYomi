using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataLib;
using Microsoft.EntityFrameworkCore;

namespace HonYomi.Core
{
    public class DirectoryScanner
    {
        private static readonly string[] audioExtensions = {".m4a", ".mp3", ".ogg", ".opus", ".wav"};

        internal static IEnumerable<ScannedBook> ScanDirectory(string path)
        {
            path = Path.GetFullPath(path);
            if (Directory.Exists(path))
            {
                List<ScannedFile> files = Directory.GetFiles(path).OrderBy(x => x).Select(ScanFile)
                    .Where(x => audioExtensions.Contains(x.Extension)).ToList();
                if (files.Any())
                {
                    yield return new ScannedBook(path, Path.GetFileName(path), files);
                }

                foreach (string subDirectory in Directory.GetDirectories(path))
                {
                    foreach (ScannedBook subBook in ScanDirectory(subDirectory))
                    {
                        yield return subBook;
                    }
                }
            }
        }

        private static ScannedFile ScanFile(string path, int index)
        {
            return new ScannedFile(path, Path.GetFileNameWithoutExtension(path), Path.GetExtension(path), index, GetMimeType(Path.GetExtension(path)));
        }

        private static string GetMimeType(string extension)
        {
            switch (extension)
            {
                case ".aac": return "audio/aac";
                case ".mp4":
                case ".m4a": return "audio/mp4";
                case ".mp1":
                case ".mp2":
                case ".mp3":
                case ".mpg":
                case ".mpeg": return "audio/mpeg";
                case ".ogg":
                case ".oga": return "audio/ogg";
                case ".wav":  return "audio/wav";
                case ".webm": return "audio/webm";
                default:      return "audio/mpeg";
            }
        }

        public static  async Task ScanWatchDirectories()
        {
            using (var db = new HonyomiContext())
            {
                var scanned = db.WatchDirectories.Select(x => x.Path).AsEnumerable().SelectMany(ScanDirectory);
                await db.InsertNewBooks(scanned);
            }
        }
    }

    internal class ScannedBook
    {
        public string            Path  { get; }
        public string            Name  { get; }
        public List<ScannedFile> Files { get; }

        public ScannedBook(string path, string name, List<ScannedFile> files)
        {
            Path  = path;
            Name  = name;
            Files = files;
        }
    }

    internal class ScannedFile
    {
        public string Path      { get; }
        public string Name      { get; }
        public string Extension { get; }
        public string MimeType { get; set; }
        public int    Index     { get; }

        public ScannedFile(string path, string name, string extension, int index, string mime)
        {
            Path      = path;
            Name      = name;
            Extension = extension;
            Index     = index;
            MimeType = mime;
        }
    }
}