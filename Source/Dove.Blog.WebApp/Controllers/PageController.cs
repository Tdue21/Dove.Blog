using Dove.Blog.Logic;
using Dove.Blog.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Dove.Blog.WebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PagesController(PageDataProvider pageDataProvider) : ControllerBase
{
    private readonly PageDataProvider _pageDataProvider = pageDataProvider ?? throw new ArgumentNullException(nameof(pageDataProvider));

    [HttpGet("list")]
    public async Task<ActionResult> PagesAsync()
    {
        var result = await _pageDataProvider.GetPages();
        return Ok(result);
    }

    [HttpGet("{pageName}")]
    public async Task<ActionResult> Get(string pageName)
    {
        var result = await _pageDataProvider.GetPage(pageName);
        return Ok(result);
    }
    /*
    [HttpPost]
    public void Post([FromBody]string value)
    {
    }

    [HttpPut("{id}")]
    public void Put(int id, [FromBody]string value)
    {
    }

    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }*/
}
