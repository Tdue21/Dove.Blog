using Westwind.AspNetCore.Markdown;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Dove.Blog.Abstractions;
using Dove.Blog.Data;
using Dove.Blog.Logic;
using Microsoft.Extensions.Caching.Memory;
using Dove.Blog.WebApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Bind configuration section to MySettings and register it as a singleton service
builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection(CacheSettings.SectionName));

// Add services to the container.
builder.Services.AddRazorPages()
                .AddRazorPagesOptions(options =>
                {
                    //options.Conventions.AddPageRoute("/About",  "Content/About");
                })
                ;

builder.Services.AddTransient<IDataProvider, FileDataProvider>();
builder.Services.AddTransient<PageDataProvider>();

builder.Services.AddMemoryCache(options =>
{
    var cacheSettings = builder.Configuration.GetSection(CacheSettings.SectionName).Get<CacheSettings>() ?? CacheSettings.Default;
    options.ExpirationScanFrequency = cacheSettings.ExpirationScanFrequency;
});

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
