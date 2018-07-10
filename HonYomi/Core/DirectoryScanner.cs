using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                List<ScannedFile> files = Directory.GetFiles(path).OrderBy(x => x).Select(ScanFile).Where(x => audioExtensions.Contains(x.Extension)).ToList();
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
            return new ScannedFile(path, Path.GetFileNameWithoutExtension(path), Path.GetExtension(path), index);
        }
    }

    internal class ScannedBook
    {
        public string Path { get; }
        public string Name { get; }
        public List<ScannedFile> Files { get; }

        public ScannedBook(string path, string name, List<ScannedFile> files)
        {
            Path = path;
            Name = name;
            Files = files;
        }
    }

    internal class ScannedFile
    {
        public string Path { get; }
        public string Name { get; }
        public string Extension { get; }
        public int Index { get; }

        public ScannedFile(string path, string name, string extension, int index)
        {
            Path = path;
            Name = name;
            Extension = extension;
            Index = index;
        }
    }
}