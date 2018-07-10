using System;

namespace DataLib
{
    public class FileProgress
    {
        public Guid         FileProgressId { get; set; }
        public Guid         UserId         { get; set; }
        public User        User           { get; set; }
        public Guid         FileId         { get; set; }
        public IndexedFile File           { get; set; }
        public long        Progress       { get; set; }
    }
}