using Dove.Blog.Abstractions;
using Dove.Blog.Logic;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Dove.Blog.Tests;

public class CategoryProviderTests
{
    [Fact]
    public async Task GetCategories_Success_TestAsync()
    {
        var memoryCache = Substitute.For<IMemoryCache>();
        var logger = Substitute.For<ILogger<BlogProvider>>();

        var fileProvider = Substitute.For<IDataProvider>();
        fileProvider.GetFileList(Arg.Any<string>()).Returns(["First-Blog-Post", "Second-Blog-Post", "Third-Blog-Post"]);
        fileProvider.ReadPageContent(Arg.Is<string>(x => x == "Posts/First-Blog-Post")).Returns(FirstBlogPost);
        fileProvider.ReadPageContent(Arg.Is<string>(x => x == "Posts/Second-Blog-Post")).Returns(SecondBlogPost);
        fileProvider.ReadPageContent(Arg.Is<string>(x => x == "Posts/Third-Blog-Post")).Returns(ThirdBlogPost);
        
        var provider = new BlogProvider(fileProvider, memoryCache, logger);
        var categories = await provider.GetCategories();

        categories.Should().NotBeNull();
        categories.Should().HaveCount(4);
        categories.Should().BeEquivalentTo([("General", 2), ("Yahoo", 1), ("Recipes", 1), ("Test", 1)]);
    }

    private const string FirstBlogPost = @"---
title: First Post
categories:
- General
---
Bla bla bla";

    private const string SecondBlogPost = @"---
title: Second Post
categories:
- Yahoo
- Recipes
---
Bla bla bla";

    private const string ThirdBlogPost = @"---
title: Third Post
categories:
- General
- Test
---
Bla bla bla";
}
