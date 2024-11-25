namespace Dove.Blog.Abstractions
{
    public interface IDataProvider
    {
        string GetMarkdown(string? pageName);
    }
}
