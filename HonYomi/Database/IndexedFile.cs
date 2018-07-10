namespace DataLib
{
    public class IndexedFile
    {
        public int         IndexedFileId { get; set; }
        public int         TrackIndex    { get; set; }
        public string      Title         { get; set; }
        public string      Filename      { get; set; }
        public string      FilePath      { get; set; }
        public int         BookId        { get; set; }
        public IndexedBook Book          { get; set; }
    }
}