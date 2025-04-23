using Dove.Blog.Data;
using Microsoft.AspNetCore.Components;

namespace Dove.Blog.WebApp.Components.Parts;
public partial class BlogPostListBase : ComponentBase
{
    [Parameter] public IEnumerable<PostSummary> Posts { get; set; } = null!;
}
