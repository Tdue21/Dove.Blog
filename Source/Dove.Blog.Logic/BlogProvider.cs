using Dove.Blog.Abstractions;
using Markdig.Extensions.Yaml;
using Markdig;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using Dove.Blog.Data;
using Microsoft.Extensions.Caching.Memory;
using Markdig.Syntax;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.RegularExpressions;

namespace Dove.Blog.Logic;

public interface IBlogProvider
{
    Task<IEnumerable<(string category, int posts)>> GetCategories();
    Task<Post> GetPost(string? postTitle);
    Task<IEnumerable<string>> GetTags();
}

public class BlogProvider(IDataProvider dataProvider, IMemoryCache memoryCache, ILogger<BlogProvider> logger) : IBlogProvider
{
    protected readonly IDataProvider _provider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
    protected readonly IMemoryCache _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    private readonly ILogger<BlogProvider> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private static Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

    public async Task<IEnumerable<string>> GetTags()
    {
        try
        {
            var files = await _provider.GetFileList("Posts");
            var posts = files.Select(x => GetPost(x).Result)
                             .ToList();

            var tags = posts.Where(p => p.Tags != null)
                            .SelectMany(p => p.Tags!)
                            .Distinct()
                            .ToArray();
            return tags;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tags");
            throw;
        }
    }

    public async Task<IEnumerable<(string category, int posts)>> GetCategories()
    {
        try
        {
            var result = new Dictionary<string, int>();

            var files = await _provider.GetFileList("Posts");
            foreach (var fileName in files)
            {
                var post = await GetPost(fileName);
                if (post.Categories != null)
                {
                    foreach (var category in post.Categories)
                    {
                        if (result.ContainsKey(category))
                        {
                            result[category]++;
                        }
                        else
                        {
                            result.Add(category, 1);
                        }
                    }
                }
            }

            return result.Select(x => (x.Key, x.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading categories");
            throw;
        }
    }

    public async Task<IEnumerable<PostSummary>> GetPosts(string? category = null, params string[] tags)
    {
        try
        {
            var files = await _provider.GetFileList("Posts");
            var posts = files.Select(x => GetPost(x).Result)
                             .ToList();

            if (!string.IsNullOrEmpty(category))
            {
                posts = posts.Where(p => p.Categories != null && p.Categories.Contains(category))
                             .ToList();
            }
            if (tags.Length > 0)
            {
                posts = posts.Where(p => p.Tags != null && p.Tags.Intersect(tags).Any())
                             .ToList();
            }



            return posts.Select(p =>
            {
                return new PostSummary
                {
                    Slug = p.Slug,
                    Title = p.Title,
                    Summary = p.Summary ?? _htmlRegex.Replace(p.Content?.Substring(0, 200) + " ...", string.Empty),
                    Author = p.Author,
                    Posted = p.Posted,
                    Updated = p.Updated
                };
            });

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading posts");
            throw;
        }
    }

    public async Task<Post> GetPost(string? postTitle)
    {
        ArgumentNullException.ThrowIfNull(postTitle, nameof(postTitle));
        try
        {
            Post? postObject = (_cache.TryGetValue("Post/" + postTitle!, out var page) ? page : null) as Post;
            if (postObject != null)
            {
                return postObject;
            }

            var markdown = await _provider.ReadPageContent($"Posts/{postTitle}");
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

                postObject = deserializer.Deserialize<Post>(yaml);
            }

            postObject ??= new Post
            {
                Slug = Slugify(postTitle),
                Author = string.Empty,
                Posted = DateTimeOffset.Now,
                Title = postTitle,
            };

            postObject.Content = document.ToHtml();

            _cache.Set("Post/" + postTitle!, postObject);
            return postObject;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading post {postTitle}", postTitle);
            throw;
        }
    }

    private string Slugify(string text)
    {
        var cleaned = Regex.Replace(text, @"\s+", " ");
        var trimmed = cleaned.Replace(" ", "-");
        var normalized = trimmed.Normalize(NormalizationForm.FormD);
        var finalized = Regex.Replace(normalized, @"[^a-zA-Z0-9\-\._]", string.Empty);

        return finalized.ToLower();
    }

    private static string RemoveHTMLTagsCompiled(string html)
    {
        return _htmlRegex.Replace(html, string.Empty);
    }

}
