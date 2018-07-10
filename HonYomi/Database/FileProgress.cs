namespace DataLib
{
    public class FileProgress
    {
        public int         FileProgressId { get; set; }
        public int         UserId         { get; set; }
        public User        User           { get; set; }
        public int         FileId         { get; set; }
        public IndexedFile File           { get; set; }
        public long        Progress       { get; set; }
    }
}