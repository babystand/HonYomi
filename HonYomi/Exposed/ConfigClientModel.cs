using System;

namespace HonYomi.Exposed
{
    public class ConfigClientModel
    {
        public bool                 WatchForChanges  { get; set; }
        public int                  ScanInterval     { get; set; }
        public int                  ServerPort       { get; set; }
        public WatchDirClientModel[] WatchDirectories { get; set; }
        public ConfigClientModel(bool watchForChanges, int scanInterval, int serverPort, WatchDirClientModel[] watchDirectories)
        {
            WatchForChanges = watchForChanges;
            ScanInterval = scanInterval;
            ServerPort = serverPort;
            WatchDirectories = watchDirectories;
        }
    }

    public class WatchDirClientModel
    {
        public string Path { get; set; }
        public Guid Guid { get; set; }
        public WatchDirClientModel(string path, Guid guid)
        {
            Path = path;
            Guid = guid;
        }
    }
}