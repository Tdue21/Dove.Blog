namespace Dove.Blog.Data;

public class Post
{
    public required string Title { get; set; }
    public required string Slug { get; set; }
    public string? Summary { get; set; }
    public string? Content { get; set; }
    public required string Author { get; set; }
    public string[]? Categories { get; set; }
    public string[]? Tags { get; set; }
    public required DateTimeOffset Posted { get; set; }
    public DateTimeOffset? Updated { get; set; }
}
