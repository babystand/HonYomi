using System;
using Microsoft.AspNetCore.Identity;

namespace DataLib
{
    public class FileProgress
    {
        public Guid         FileProgressId { get; set; }
        public string         UserId         { get; set; }
        public IdentityUser       User           { get; set; }
        public Guid         FileId         { get; set; }
        public IndexedFile File           { get; set; }
        public uint        Progress       { get; set; } //in seconds
    }
}