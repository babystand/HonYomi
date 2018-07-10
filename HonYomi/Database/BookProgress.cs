using System;

namespace DataLib
{
    public class BookProgress
    {
        public Guid         BookProgressId { get; set; }
        public Guid         UserId         { get; set; }
        public User        User           { get; set; }
        public Guid         BookId         { get; set; }
        public IndexedBook Book           { get; set; }
        public Guid         FileId         { get; set; }
        public IndexedFile File           { get; set; }
    }
}