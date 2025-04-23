using Dove.Blog.WebApp.Models;
using Microsoft.AspNetCore.Components;

namespace Dove.Blog.WebApp.Components.Layout;

public class PageDataComponent : ComponentBase
{
    [Parameter] public PageDataDto Data { get; set; } = null!;
}
