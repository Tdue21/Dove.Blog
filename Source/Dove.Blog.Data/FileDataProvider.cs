using Dove.Blog.Abstractions;
using Microsoft.AspNetCore.Hosting;
using System.Text;

namespace Dove.Blog.Data;

public class FileDataProvider(IHostingEnvironment environment) : IDataProvider
{
    private readonly IHostingEnvironment _environment = environment ?? throw new ArgumentNullException(nameof(environment));
    private string? _rootPath = null;

    public string RootPath
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_rootPath))
            {
                var rootPath = _environment.WebRootPath;
                var filePath = Path.Combine(rootPath, "data");
                _rootPath = filePath;
            }
            return _rootPath;
        }
    }

    public Task<string> ReadPageContent(string pageName, Encoding encoding = null!)
    {
        var filePath = Path.Combine(RootPath, pageName + ".md");
        var content = File.ReadAllTextAsync(filePath, encoding ?? Encoding.UTF8);
        return content;
    }

    public Task WritePageContent(string pageName, string content, Encoding encoding = null!)
    {
        var filePath = Path.Combine(RootPath, pageName + ".md");
        return File.WriteAllTextAsync(filePath, content, encoding ?? Encoding.UTF8);
    }

    //public Task<string> GetMarkdown(string? pageName)
    //{
    //    Page? pageObject = null;

    //    var rootPath = _environment.WebRootPath;
    //    var filePath = Path.Combine(rootPath, "data", "Pages", pageName + ".md");

    //    var pipeline = new MarkdownPipelineBuilder().UseYamlFrontMatter().Build();

    //    var markdown = File.ReadAllText(filePath);
    //    var document = Markdown.Parse(markdown, pipeline);
    //    var yamlBlock = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();
    //    if (yamlBlock != null)
    //    {
    //        var yaml = markdown.Substring(yamlBlock.Span.Start + 3, yamlBlock.Span.Length - 6);
    //        markdown = markdown.Substring(yamlBlock.Span.Length + 1);
    //        document = Markdown.Parse(markdown, pipeline);

    //        var deserializer = new DeserializerBuilder()
    //                .WithNamingConvention(UnderscoredNamingConvention.Instance)
    //                .Build();
    //        pageObject = deserializer.Deserialize<Page>(yaml);
    //    }

    //    if (pageObject == null)
    //    {
    //        pageObject = new Page
    //        {
    //            Author = string.Empty,
    //            Created = DateTimeOffset.Now,
    //            Title = pageName ?? string.Empty,
    //        };
    //    }
    //    pageObject.Content = document.ToHtml();

    //    return Task.FromResult(document.ToHtml()); // Westwind.AspNetCore.Markdown.ParseFromFileAsync($"~/data/Pages/{pageName}.md", sanitizeHtml: true);
    //}
}
