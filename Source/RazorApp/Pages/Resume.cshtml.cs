using Dove.Blog.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dove.Blog.WebApp.Pages;

public class ResumeModel(IDataProvider dataProvider, ILogger<ResumeModel> logger) : PageModel
{
    private readonly IDataProvider _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
    private readonly ILogger<ResumeModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public string? PageContent { get; private set; }

    public async Task OnGet()
    {
       // PageContent = await _dataProvider.GetMarkdown("Resume");
    }
}
