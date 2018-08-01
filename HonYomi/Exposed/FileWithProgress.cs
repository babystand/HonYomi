using System;

namespace HonYomi.Exposed
{
    public class FileWithProgress
    {
        public Guid Guid { get; set; }
        public string Title { get; set; }
        public Guid BookGuid { get; set; }
        public string BookTitle { get; set; }
        public int TrackIndex {get;set;}
        public uint ProgressSeconds { get; set; }
        public string MediaType { get; set; }
    }
}