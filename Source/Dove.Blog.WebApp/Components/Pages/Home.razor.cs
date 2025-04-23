using Dove.Blog.Logic;
using Dove.Blog.WebApp.Models;
using Microsoft.AspNetCore.Components;

namespace Dove.Blog.WebApp.Components.Pages;

public class HomeComponentBase : ComponentBase
{
    [Inject] public PageDataProvider DataProvider { get; set; } = null!;
    public MarkupString? PageContent { get; private set; }
    public string? Title { get; private set; }
    public PageDataDto PageData { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        var page = await DataProvider.GetPage("Index");
        PageContent = new MarkupString(page.Content ?? "");
        Title = page.Title; 
        PageData = new(page.Created.ToString("g"), page.Updated.ToString("g"), page.Author, page.Title);

        await base.OnInitializedAsync();
    }
}
