namespace Dove.Blog.WebApp.Models;

public record PageDto(string Title, string? Summary, string? Content, string Author, string[]? Categories, DateTimeOffset? Posted, DateTimeOffset? Updated);

