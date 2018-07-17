using System;
using System.IO;
using System.Runtime.CompilerServices;
[assembly:InternalsVisibleTo("HonYomi.Tests")]

public static class RuntimeConstants
{
    public static readonly string DataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config",
                                                "honyomi");
    public static readonly string DatabaseLocation = Path.Combine(DataDir, "honyomi.db");
    public static readonly string TempDir = Path.Combine(DataDir, "temp");
    public static readonly string JwtKey = "VGhlIFdoZWVsIG9mIFRpbWUgdHVybnMsIGFuZCBBZ2VzIGNvbWUgYW5kIHBhc3MsIGxlYXZpbmcgbWVtb3JpZXMgdGhhdCBiZWNvbWUgbGVnZW5kLiBMZWdlbmQgZmFkZXMgdG8gbXl0aCwgYW5kIGV2ZW4gbXl0aCBpcyBsb25nIGZvcmdvdHRlbiB3aGVuIHRoZSBBZ2UgdGhhdCBnYXZlIGl0IGJpcnRoIGNvbWVzIGFnYWlu";
    public static readonly string JwtIssuer = "0.0.0.0:5000";
}