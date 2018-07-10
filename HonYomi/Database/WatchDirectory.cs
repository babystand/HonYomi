using System;

namespace DataLib
{
    public class WatchDirectory
    {
        public Guid    WatchDirectoryId { get; set; }
        public string Path             { get; set; }
        public Guid ConfigId { get; set; }
        public HonyomiConfig Config { get; set; }
    }
}