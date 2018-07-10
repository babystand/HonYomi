namespace DataLib
{
    public class BookProgress
    {
        public int         BookProgressId { get; set; }
        public int         UserId         { get; set; }
        public User        User           { get; set; }
        public int         BookId         { get; set; }
        public IndexedBook Book           { get; set; }
        public int         FileId         { get; set; }
        public IndexedFile File           { get; set; }
    }
}