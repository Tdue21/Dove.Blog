using Dove.Blog.Logic;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dove.Blog.WebApp.Pages;

public class ResumeModel(PageDataProvider dataProvider, ILogger<ResumeModel> logger) : PageModel
{
    private readonly PageDataProvider _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
    private readonly ILogger<ResumeModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public string? PageContent { get; private set; }
    public DateTimeOffset? Created { get; private set; }
    public DateTimeOffset? Updated { get; private set; }
    public string? Author { get; private set; }
    public string? Title { get; private set; }

    public async Task OnGet()
    {
        var page = await _dataProvider.GetPage("Resume");
        PageContent = page.Content;
        Created = page.Created;
        Updated = page.Updated;
        Author = page.Author;
        Title = page.Title;
    }
}
