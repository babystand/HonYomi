using System.Collections.Generic;

namespace DataLib
{
    public class HonyomiConfig
    {
        public int  HonyomiConfigId  { get; set; }
        public bool WatchForChanges { get; set; }
        public int  ScanInterval    { get; set; }
        public int  ServerPort      { get; set; }
        public List<WatchDirectory> WatchDirectories { get; set; }
    }
}