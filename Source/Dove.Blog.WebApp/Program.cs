using Westwind.AspNetCore.Markdown;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Dove.Blog.Abstractions;
using Dove.Blog.Data;
using Dove.Blog.Logic;
using Dove.Blog.WebApp.Components;
using Dove.Blog.WebApp.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog();

    // Bind configuration section to MySettings and register it as a singleton service
    builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection(CacheSettings.SectionName));

    // Add services to the container.
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    builder.Services.AddTransient<IDataProvider, FileDataProvider>();
    builder.Services.AddTransient<PageDataProvider>();
    builder.Services.AddTransient<BlogProvider>();

    builder.Services.AddMemoryCache(options =>
    {
        var cacheSettings = builder.Configuration.GetSection(CacheSettings.SectionName).Get<CacheSettings>() ?? CacheSettings.Default;
        options.ExpirationScanFrequency = cacheSettings.ExpirationScanFrequency;
    });

    builder.Services.AddMarkdown(config =>
    {
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
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddHttpLogging(logging =>
    {
        logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties;
        logging.CombineLogs = true;
    });

    var app = builder.Build();
    app.UseHttpLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "v1");
        });
    }

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    app.UseStatusCodePagesWithRedirects("/Error/{0}");
    app.UseHttpsRedirection();
    app.UseMarkdown();
    app.UseAntiforgery();
    app.MapStaticAssets();
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();
    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}