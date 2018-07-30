using System;

namespace DataLib
{
    public class TempMediaLocation
    {
        public Guid Id { get; set; }
        public DateTime Expires { get; set; }
        public Guid FileId { get; set; }
        public IndexedFile File { get; set; }
    }
}