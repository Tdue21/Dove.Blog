using Dove.Blog.Abstractions;
using Dove.Blog.Data;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Microsoft.Extensions.Caching.Memory;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Dove.Blog.Logic;

public class PageDataProvider(IDataProvider provider, IMemoryCache memoryCache) 
{
    private readonly IDataProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    private readonly IMemoryCache _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

    public async Task<IEnumerable<Page>> GetPages()
    {
        var pages = new List<Page>();
        var pageNames = await _provider.GetFileList("Pages");
        foreach (var pageName in pageNames)
        {
            var page = await GetPageFrontMatter(pageName);
            if (page != null)
            {
                pages.Add(page);
            }
        }
        return pages;
    }

    public async Task<Page> GetPage(string? pageName)
    {
        ArgumentNullException.ThrowIfNull(pageName, nameof(pageName));

        Page? pageObject = (_cache.TryGetValue("Page/" + pageName!, out var page) ? page : null) as Page;
        if(pageObject != null)
        {
            return pageObject;
        }

        var markdown = await _provider.ReadPageContent($"Pages/{pageName}");
        var pipeline = new MarkdownPipelineBuilder()
                            .UseYamlFrontMatter()
                            .UseEmojiAndSmiley()
                            .Build();

        var document = Markdown.Parse(markdown, pipeline);
        var yamlBlock = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();
        if (yamlBlock != null)
        {
            var yaml = markdown.Substring(yamlBlock.Span.Start + 3, yamlBlock.Span.Length - 6);
            markdown = markdown.Substring(yamlBlock.Span.Length + 1);
            document = Markdown.Parse(markdown, pipeline);

            var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(UnderscoredNamingConvention.Instance)
                    .Build();

            pageObject = deserializer.Deserialize<Page>(yaml);
        }

        pageObject ??= new Page
            {
                Author = string.Empty,
                Created = DateTimeOffset.Now,
                Title = pageName ?? string.Empty,
            };
        pageObject.Content = document.ToHtml();

        _cache.Set("Page/" + pageName!, pageObject);
        return pageObject;
    }

    private async Task<Page?> GetPageFrontMatter(string pageName)
    {
        ArgumentNullException.ThrowIfNull(pageName, nameof(pageName));

        var markdown = await _provider.ReadPageContent($"Pages/{pageName}");
        var pipeline = new MarkdownPipelineBuilder()
                            .UseYamlFrontMatter()
                            .Build();

        var document = Markdown.Parse(markdown, pipeline);
        var yamlBlock = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();
        if (yamlBlock != null)
        {
            var yaml = markdown.Substring(yamlBlock.Span.Start + 3, yamlBlock.Span.Length - 6);
            markdown = markdown.Substring(yamlBlock.Span.Length + 1);
            document = Markdown.Parse(markdown, pipeline);

            var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(UnderscoredNamingConvention.Instance)
                    .Build();

            var pageObject = deserializer.Deserialize<Page>(yaml);
            return pageObject;
        }

        return null;
    }
}
