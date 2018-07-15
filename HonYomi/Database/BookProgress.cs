using System;
using Microsoft.AspNetCore.Identity;

namespace DataLib
{
    public class BookProgress
    {
        public Guid         BookProgressId { get; set; }
        public string         UserId         { get; set; }
        public IdentityUser        User           { get; set; }
        public Guid         BookId         { get; set; }
        public IndexedBook Book           { get; set; }
        public Guid         FileId         { get; set; }
        public IndexedFile File           { get; set; }
    }
}