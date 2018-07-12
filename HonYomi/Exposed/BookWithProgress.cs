using System;

namespace HonYomi.Exposed
{
    public class BookWithProgress
    {
        public Guid Guid { get; set; }
        public Guid CurrentTrackGuid { get; set; }
        public string Title { get; set; }
        public FileWithProgress[] FileProgresses { get; set; }
        
    }
}