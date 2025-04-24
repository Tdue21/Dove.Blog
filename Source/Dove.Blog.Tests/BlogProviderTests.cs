using Dove.Blog.Abstractions;
using Dove.Blog.Data;
using Dove.Blog.Logic;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using Xunit;

namespace Dove.Blog.Tests;

public class BlogProviderTests
{
    private readonly BlogProvider _provider;

    public BlogProviderTests()
    {
        var memoryCache = Substitute.For<IMemoryCache>();
        var logger = Substitute.For<ILogger<BlogProvider>>();

        var fileProvider = Substitute.For<IDataProvider>();
        fileProvider.GetFileList(Arg.Any<string>()).Returns(["First-Blog-Post", "Second-Blog-Post", "Third-Blog-Post"]);
        fileProvider.ReadPageContent(Arg.Is<string>(x => x == "Posts/First-Blog-Post")).Returns(FirstBlogPost);
        fileProvider.ReadPageContent(Arg.Is<string>(x => x == "Posts/Second-Blog-Post")).Returns(SecondBlogPost);
        fileProvider.ReadPageContent(Arg.Is<string>(x => x == "Posts/Third-Blog-Post")).Returns(ThirdBlogPost);

        _provider = new BlogProvider(fileProvider, memoryCache, logger);
    }

    [Fact]
    public async Task GetCategories_Success_TestAsync()
    {
        var categories = await _provider.GetCategories();

        categories.Should().NotBeNull();
        categories.Should().HaveCount(4);
        categories.Should().BeEquivalentTo([("General", 2), ("Yahoo", 1), ("Recipes", 1), ("Test", 1)]);
    }

    [Fact]
    public async Task GetTags_Success_TestAsync()
    {
        var tags = await _provider.GetTags();
        tags.Should().NotBeNull();
        tags.Should().HaveCount(2);
        tags.Should().BeEquivalentTo(["blog", "test"]);
    }

    [Fact]
    public async Task GetPosts_No_Filters_Success_TestAsync()
    {
        var posts = await _provider.GetPosts();
        posts.Should().NotBeNull();
        posts.Should().HaveCount(3);
    }


    [Fact]
    public async Task GetPosts_Filter_On_Category_Success_TestAsync()
    {
        var posts = await _provider.GetPosts("General");
        posts.Should().NotBeNull();
        posts.Should().HaveCount(2);
    }


    [Fact]
    public async Task GetPosts_Filter_On_Tags_Success_TestAsync()
    {
        var posts = await _provider.GetPosts(category: null, "test", "blog");
        posts.Should().NotBeNull();
        posts.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetPost_Success_TestAsync()
    {
        var expected = new Post
        {
            Title = "First Post",
            Author = "tdue21",
            Slug = "first-blog-post",
            Categories = ["General"],
            Tags = ["blog", "test"],
            Content = "<p>Bla bla bla</p>\n",
            Posted = new DateTimeOffset(2023, 10, 1, 0, 0, 0, TimeSpan.Zero),
        };
        var post = await _provider.GetPost("First-Blog-Post");
        post.Should().NotBeNull();
        post.Should().BeEquivalentTo(expected, options => options
            .Excluding(x => x.Posted)
            .Excluding(x => x.Updated)
            .Excluding(x => x.Summary));
        // Timezone difference can be up to 2 hours
        post.Posted.Should().BeCloseTo(expected.Posted, TimeSpan.FromHours(2));
    }

    private const string FirstBlogPost = @"---
title: First Post
author: tdue21
slug: first-blog-post
posted: 2023-10-01
categories:
- General
tags:
- blog
- test
---
Bla bla bla";

    private const string SecondBlogPost = @"---
title: Second Post
categories:
- Yahoo
- Recipes
tags:
- blog
---
Bla bla bla";

    private const string ThirdBlogPost = @"---
title: Third Post
categories:
- General
- Test
tags:
- test
---
Bla bla bla";
}
