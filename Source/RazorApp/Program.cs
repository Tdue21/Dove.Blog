using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Westwind.AspNetCore.Markdown;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options => 
    {
        options.Conventions.AddPageRoute("/Pages", "Pages/Content/{pageName}");
    });

builder.Services.AddMarkdown();

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
