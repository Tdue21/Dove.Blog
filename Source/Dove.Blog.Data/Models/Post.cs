namespace Dove.Blog.Data;

public class Post
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Author { get; set; }
    public string? Category { get; set; }
    public string? UpdatedBy { get; set; }
    public required string FileName { get; set; }
    public required DateTimeOffset Created { get; set; }
    public required DateTimeOffset Updated { get; set; }
}