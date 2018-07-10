namespace DataLib
{
    public class WatchDirectory
    {
        public int    WatchDirectoryId { get; set; }
        public string Path             { get; set; }
        public int ConfigId { get; set; }
        public HonyomiConfig Config { get; set; }
    }
}