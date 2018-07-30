using System;

namespace HonYomi.Exposed
{
    public class FileReservation
    {
        public string Url { get; set; }
        public Guid FileId { get; set; }
        public string MimeType { get; set; }
    }
}