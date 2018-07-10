using System.Collections.Generic;

namespace DataLib
{
    public class IndexedBook
    {
        public int               IndexedBookId { get; set; }
        public string            Title         { get; set; }
        public string            DirectoryPath { get; set; }
        public List<IndexedFile> Files         { get; set; }
    }
}