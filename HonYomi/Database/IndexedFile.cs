using System;

namespace DataLib
{
    public class IndexedFile
    {
        public Guid         IndexedFileId { get; set; }
        public int         TrackIndex    { get; set; }
        public string      Title         { get; set; }
        public string      Filename      { get; set; }
        public string      FilePath      { get; set; }
        public TimeSpan Duration { get; set; }
        public string MimeType { get; set; }
        public Guid         BookId        { get; set; }
        public IndexedBook Book          { get; set; }
    }
}