using Microsoft.AspNetCore.Mvc.RazorPages;
using Westwind.AspNetCore.Markdown;

namespace Dove.Blog.WebApp.Pages
{
    public class ResumeModel : PageModel
    {
        public string? PageContent { get; private set; }
     
        public async Task OnGet()
        {
            PageContent = await Markdown.ParseFromFileAsync($"~/data/Pages/Resume.md", sanitizeHtml: true);            
        }
    }
}
