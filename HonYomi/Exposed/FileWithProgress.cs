using System;
using DataLib;

namespace HonYomi.Exposed
{
    public class FileWithProgress
    {
        public Guid   Guid            { get; set; }
        public string Title           { get; set; }
        public Guid   BookGuid        { get; set; }
        public string BookTitle       { get; set; }
        public int    TrackIndex      { get; set; }
        public double ProgressSeconds { get; set; }
        public string MediaType       { get; set; }
        public Guid NextFile { get; set; }


        public FileWithProgress()
        {
        }

        public FileWithProgress(Guid guid,            string title, Guid bookGuid, string bookTitle, int trackIndex,
            double                   progressSeconds, string mediaType)
        {
            Guid            = guid;
            Title           = title;
            BookGuid        = bookGuid;
            BookTitle       = bookTitle;
            TrackIndex      = trackIndex;
            ProgressSeconds = progressSeconds;
            MediaType       = mediaType;
        }
    }
}