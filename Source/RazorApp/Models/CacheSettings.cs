namespace Dove.Blog.WebApp.Models;

public class CacheSettings
{
    public const string SectionName = "CacheSettings";

    public int SizeLimit { get; set; }
    public TimeSpan ExpirationScanFrequency { get; set; }

    public static CacheSettings Default => new CacheSettings
    {
        SizeLimit = 1000,
        ExpirationScanFrequency = TimeSpan.FromMinutes(10)
    };  
}
