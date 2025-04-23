namespace Dove.Blog.WebApp.Models;

public class PostDto
{
    public required string Title { get; set; }
    public string? Summary { get; set; }
    public string? Content { get; set; }
    public required string Author { get; set; }
    public string[]? Categories { get; set; }
    public DateTimeOffset? Posted { get; set; }
    public DateTimeOffset? Updated { get; set; }
}
