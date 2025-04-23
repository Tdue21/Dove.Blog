using Dove.Blog.Data;
using Microsoft.AspNetCore.Components;

namespace Dove.Blog.WebApp.Components.Parts;

public partial class BlogContentBase : ComponentBase
{
    [Parameter]public Post? CurrentPost { get; set; }
    [Parameter] public MarkupString? PageContent { get; set; }

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
}
