namespace Dove.Blog.WebApp.Models;

public record PostDto(string Title, string? Summary, string? Content, string Author, string[]? Categories, DateTimeOffset? Posted, DateTimeOffset? Updated);

