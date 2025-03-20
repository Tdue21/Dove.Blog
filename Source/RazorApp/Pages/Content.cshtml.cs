using Dove.Blog.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Westwind.AspNetCore.Markdown;

namespace Dove.Blog.WebApp.Pages
{
    public class ContentModel : PageModel
    {
        public string? PageName { get; set;}
        public string? PageContent { get; private set; }

        public async Task OnGet()
        {
            var pageName = RouteData.Values["pageName"]?.ToString();

            PageContent = await Markdown.ParseFromFileAsync($"~/data/Pages/{pageName}.md", sanitizeHtml: true);
            
            ViewData["Title"] = pageName;
            PageName = pageName?.ToString();
            Console.WriteLine(pageName);


            
        }
    }
}
