namespace DataLib
{
    public class User
    {
        public int    UserId     { get; set; }
        public string Username   { get; set; }
        public string HashedPass { get; set; }
        public string HashSalt   { get; set; }
        public bool   IsAdmin    { get; set; }
    }
}