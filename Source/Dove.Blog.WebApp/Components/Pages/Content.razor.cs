using Dove.Blog.Logic;
using Dove.Blog.WebApp.Models;
using Microsoft.AspNetCore.Components;

namespace Dove.Blog.WebApp.Components.Pages;

public class ContentPageBase : ComponentBase
{
    [Inject] public ILogger<BlogPage> Logger { get; set; } = null!;
    [Inject] public PageDataProvider DataProvider { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Parameter] public string PageName { get; set; } = null!;

    public string CurrentName { get; set; } = null!;

    public MarkupString? PageContent { get; private set; }
    public string? Title { get; private set; }
    public PageDataDto? PageData { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (!string.IsNullOrEmpty(PageName))
            {
                try
                {
                    var page = await DataProvider.GetPage(PageName);
                    PageContent = new MarkupString(page.Content ?? "");
                    Title = page.Title;
                    CurrentName = PageName;
                    PageData = new(page.Created.ToString("g"), page.Updated.ToString("g"), page.Author, page.Title);
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
                    NavigationManager.NavigateTo("/");
                }
            }
            else
            {
                NavigationManager.NavigateTo("/");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading page {PageName}", PageName);
            throw;
        }
        await base.OnInitializedAsync();
    }

    protected override Task OnParametersSetAsync()
    {
        if (!PageName.Equals(CurrentName, StringComparison.InvariantCultureIgnoreCase))
        {
            return OnInitializedAsync();
        }
        return base.OnParametersSetAsync();
    }
}
