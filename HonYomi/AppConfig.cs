using System;
using System.IO;
using System.Runtime.CompilerServices;
[assembly:InternalsVisibleTo("HonYomi.Tests")]

public static class RuntimeConstants
{
    public static readonly string DataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config",
                                                "onyomi");
    public static readonly string DatabaseLocation = Path.Combine(DataDir, "onyomi.db");
    public static readonly string TempDir = Path.Combine(DataDir, "temp");
}