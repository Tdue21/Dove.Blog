using Dove.Blog.Data;
using Dove.Blog.Logic;
using Microsoft.AspNetCore.Components;

namespace Dove.Blog.WebApp.Components.Pages;

public class BlogPage : ComponentBase
{
    [Inject] public ILogger<BlogPage> Logger { get; set; } = null!;
    [Inject] public BlogProvider BlogProvider { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;

    [Parameter] public string? Slug { get; set; } = null;
    [Parameter] public string? Category { get; set; } = null;
    [Parameter] public string? Tag { get; set; } = null;

    public bool IsLoading { get; set; } = true;
    public string? CurrentSlug { get; set; } = null;
    public string? PreviousSlug { get; set; } = null;
    public string? NextSlug { get; set; } = null;
    public string? Title { get; private set; }
    public MarkupString? PageContent { get; private set; }
    public Post? CurrentPost { get; set; }

    public IEnumerable<string> Categories { get; private set; } = Enumerable.Empty<string>();
    public IEnumerable<string> Tags { get; private set; } = Enumerable.Empty<string>();
    public IEnumerable<PostSummary> Posts { get; private set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        try
        {
            Categories = (await BlogProvider.GetCategories()).Select(x => x.category);
            Tags = await BlogProvider.GetTags();

            await LoadPageData();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading post {Slug}", Slug);
            throw;
        }
        finally
        {
            IsLoading = false;
        }

        await base.OnInitializedAsync();
    }

    protected MarkupString GetCategories()
    {
        var categories = CurrentPost?.Categories?.Select(x => $"<a href=\"/Blog/Categories/{x}\">{x}</a>") ?? Enumerable.Empty<string>();
        return new MarkupString(string.Join(", ", categories));
    }

    protected MarkupString GetTags()
    {
        var tags = CurrentPost?.Tags?.Select(x => $"<a href=\"/Blog/Tags/{x}\">{x}</a>") ?? Enumerable.Empty<string>();
        return new MarkupString(string.Join(", ", tags));
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!IsLoading && (Slug == null || !string.Equals(Slug, CurrentSlug)))
        {
            IsLoading = true;
            try
            {
                await LoadPageData();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading post {Slug}", Slug);
                throw;
            }
            finally
            {
                IsLoading = false;
            }
        }
        await base.OnParametersSetAsync();
    }

    private async Task LoadPageData()
    {
        PageContent = null;
        Posts = await BlogProvider.GetPosts();
        Posts = Posts.OrderByDescending(x => x.Posted);
        if (!string.IsNullOrEmpty(Slug))
        {
            try
            {
                CurrentPost = await BlogProvider.GetPost(Slug);
                PageContent = new MarkupString(CurrentPost.Content ?? "");
                Title = CurrentPost.Title;
                CurrentSlug = Slug;

                var index = 0; 
                var indexedPosts = Posts.ToDictionary(x => index++, x => x);
                var currentPost = indexedPosts.FirstOrDefault(x => x.Value.Slug.Equals(Slug, StringComparison.InvariantCultureIgnoreCase));
                var currentIndex = currentPost.Key;

                PreviousSlug = currentIndex < Posts.Count() - 1 && currentIndex >= 0 ? indexedPosts[currentIndex + 1].Slug : null;
                NextSlug = currentIndex > 0 ? indexedPosts[currentIndex - 1].Slug : null; 
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                NavigationManager.NavigateTo("/");
            }
        }
    }
}
