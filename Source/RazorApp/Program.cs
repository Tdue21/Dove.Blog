using Westwind.AspNetCore.Markdown;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Dove.Blog.Abstractions;
using Dove.Blog.Data;
using Dove.Blog.Logic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages()
                .AddRazorPagesOptions(options =>
                {
                    //options.Conventions.AddPageRoute("/About",  "Content/About");
                })
                ;

builder.Services.AddTransient<IDataProvider, FileDataProvider>();
builder.Services.AddTransient<PageDataProvider>();

builder.Services.AddMarkdown(config =>
{
    config.AddMarkdownProcessingFolder("/data/Pages/", "~/Pages/__MarkdownPageTemplate.cshtml");
    config.ConfigureMarkdigPipeline = builder =>
        builder.UseEmphasisExtras(Markdig.Extensions.EmphasisExtras.EmphasisExtraOptions.Default)
            .UsePipeTables()
            .UseGridTables()
            .UseAutoIdentifiers(AutoIdentifierOptions.GitHub) // Headers get id="name" 
            .UseAutoLinks() // URLs are parsed into anchors
            .UseAbbreviations()
            .UseYamlFrontMatter()
            .UseEmojiAndSmiley(true)
            .UseListExtras()
            .UseFigures()
            .UseTaskLists()
            .UseCustomContainers()
            .UseGenericAttributes();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseMarkdown();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
