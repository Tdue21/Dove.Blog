using Microsoft.AspNetCore.Components;

namespace Dove.Blog.WebApp.Components.Parts;
public partial class BlogNavigationBase : ComponentBase
{
    [Parameter] public string? PreviousSlug { get; set; } = null;
    [Parameter] public string? NextSlug { get; set; } = null;
}
