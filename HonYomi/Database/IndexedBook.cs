using System;
using System.Collections.Generic;

namespace DataLib
{
    public class IndexedBook
    {
        public Guid               IndexedBookId { get; set; }
        public string            Title         { get; set; }
        public string            DirectoryPath { get; set; }
        public List<IndexedFile> Files         { get; set; }
    }
}