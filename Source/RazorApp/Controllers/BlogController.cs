using Dove.Blog.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Dove.Blog.WebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BlogController : ControllerBase
{
    [HttpGet()]
    public IEnumerable<PostDto> Categories()
    {
        return null;
    }

    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

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
    }
}
