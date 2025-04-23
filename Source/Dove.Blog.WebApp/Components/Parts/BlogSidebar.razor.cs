using Microsoft.AspNetCore.Components;

namespace Dove.Blog.WebApp.Components.Parts;
public partial class BlogSidebarBase : ComponentBase
{
    [Parameter] public IEnumerable<(string Category, int Posts)> Categories { get; set; } = Enumerable.Empty<(string Category, int Posts)>();
    [Parameter] public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();

}
