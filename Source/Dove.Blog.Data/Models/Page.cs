namespace Dove.Blog.Data;

public class Page
{
    public required string Title { get; set; }
    public string? Content { get; set; }
    public required string? Author { get; set; }
    public string? UpdatedBy { get; set; }
    public required DateTimeOffset Created { get; set; }
    public DateTimeOffset Updated { get; set; }
}
