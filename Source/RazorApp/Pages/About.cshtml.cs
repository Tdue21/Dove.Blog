using Dove.Blog.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dove.Blog.WebApp.Pages;

public class AboutModel(IDataProvider dataProvider, ILogger<AboutModel> logger) : PageModel
{
    private readonly IDataProvider _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
    private readonly ILogger<AboutModel> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public string? PageContent { get; private set; }

    public async Task OnGet()
    {
        
        //PageContent = await _dataProvider.GetMarkdown("About"); 
    }
}

