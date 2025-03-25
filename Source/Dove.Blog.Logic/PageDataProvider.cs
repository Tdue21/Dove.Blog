using System;
using Dove.Blog.Abstractions;
using Dove.Blog.Data;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Microsoft.AspNetCore.Hosting;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Dove.Blog.Logic;

public class PageDataProvider(IDataProvider provider) 
{
    private readonly IDataProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));

    public async Task<Page> GetPage(string? pageName)
    {
        Page? pageObject = null;

        var markdown = await _provider.ReadPageContent($"Pages/{pageName}");
        var pipeline = new MarkdownPipelineBuilder().UseYamlFrontMatter().UseEmojiAndSmiley().Build();
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

        return pageObject;
    }
}
