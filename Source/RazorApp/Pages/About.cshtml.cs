using Microsoft.AspNetCore.Mvc.RazorPages;
using Westwind.AspNetCore.Markdown;

namespace Dove.Blog.WebApp.Pages;

public class AboutModel(IMarkdownParserFactory parserFactory) : PageModel
{
    private readonly IMarkdownParserFactory _parserFactory = parserFactory ?? throw new ArgumentNullException(nameof(parserFactory));

    public string? PageContent { get; private set; }

    public async Task OnGet()
    {
        var parser = _parserFactory.GetParser();
        var data = parser.Parse("## Test\nBlabla\n\n**Test**");


        PageContent = await Markdown.ParseFromFileAsync($"~/data/Pages/About.md", sanitizeHtml: true);
    }
}

