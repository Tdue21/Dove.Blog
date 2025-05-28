using Dove.Blog.Abstractions;
using Dove.Blog.Data;
using Dove.Blog.Logic;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Dove.Blog.Tests;

public class PageDataProviderTests
{
    [Fact]
    public async Task GetPage_Success_TestAsync()
    {
        var fileProvider = Substitute.For<IDataProvider>();
        var memoryCache = Substitute.For<IMemoryCache>();
        fileProvider.ReadPageContent(Arg.Any<string>()).Returns("## Hello, World!");
        var provider = new PageDataProvider(fileProvider, memoryCache);
        var page = await provider.GetPage("Index");

        page.Should().NotBeNull();
        page.Content.Should().NotBeNullOrEmpty();
        page.Content.Should().BeEquivalentTo("<h2>Hello, World!</h2>\n");
    }

    [Fact]
    public async Task GetPage_Page_Not_Found_TestAsync()
    {
        var fileProvider = Substitute.For<IDataProvider>();
        var memoryCache = Substitute.For<IMemoryCache>();
        fileProvider.ReadPageContent(Arg.Any<string>()).Throws<FileNotFoundException>();

        var provider = new PageDataProvider(fileProvider, memoryCache);

        await provider.Invoking(x => x.GetPage("Index")).Should().ThrowAsync<FileNotFoundException>();
    }

    [Fact]
    public async Task GetPage_Throws_ArgumentNullException_When_PageName_Is_Null()
    {
        var fileProvider = Substitute.For<IDataProvider>();
        var memoryCache = Substitute.For<IMemoryCache>();
        var provider = new PageDataProvider(fileProvider, memoryCache);

        await Assert.ThrowsAsync<ArgumentNullException>(() => provider.GetPage(null));
    }

    [Fact]
    public async Task GetPage_Returns_From_Cache_If_Available()
    {
        var fileProvider = Substitute.For<IDataProvider>();
        var memoryCache = Substitute.For<IMemoryCache>();
        var cachedPage = new Page
        {
            Title = "Cached",
            Author = "Author",
            Created = DateTimeOffset.UtcNow
        };
        object outObj = cachedPage;
        memoryCache.TryGetValue("Page/Index", out outObj).Returns(x =>
        {
            x[1] = cachedPage;
            return true;
        });

        var provider = new PageDataProvider(fileProvider, memoryCache);

        var page = await provider.GetPage("Index");

        page.Should().BeSameAs(cachedPage);
        fileProvider.DidNotReceive().ReadPageContent(Arg.Any<string>());
    }

    [Fact]
    public async Task GetPages_Returns_All_Pages_With_FrontMatter()
    {
        var fileProvider = Substitute.For<IDataProvider>();
        var memoryCache = Substitute.For<IMemoryCache>();
        fileProvider.GetFileList("Pages").Returns(new[] { "Page1", "Page2" });
        fileProvider.ReadPageContent("Pages/Page1").Returns("---\ntitle: Page1\nauthor: Author1\ncreated: 2024-01-01T00:00:00Z\n---\nContent1");
        fileProvider.ReadPageContent("Pages/Page2").Returns("---\ntitle: Page2\nauthor: Author2\ncreated: 2024-01-02T00:00:00Z\n---\nContent2");

        var provider = new PageDataProvider(fileProvider, memoryCache);

        var pages = (await provider.GetPages()).ToList();

        pages.Should().HaveCount(2);
        pages[0].Title.Should().Be("Page1");
        pages[1].Title.Should().Be("Page2");
    }

    [Fact]
    public async Task GetPages_Skips_Pages_Without_FrontMatter()
    {
        var fileProvider = Substitute.For<IDataProvider>();
        var memoryCache = Substitute.For<IMemoryCache>();
        fileProvider.GetFileList("Pages").Returns(new[] { "Page1", "Page2" });
        fileProvider.ReadPageContent("Pages/Page1").Returns("---\ntitle: Page1\nauthor: Author1\ncreated: 2024-01-01T00:00:00Z\n---\nContent1");
        fileProvider.ReadPageContent("Pages/Page2").Returns("No front matter here");

        var provider = new PageDataProvider(fileProvider, memoryCache);

        var pages = (await provider.GetPages()).ToList();

        pages.Should().HaveCount(1);
        pages[0].Title.Should().Be("Page1");
    }

    [Fact]
    public async Task GetPages_Returns_Empty_When_No_Pages()
    {
        var fileProvider = Substitute.For<IDataProvider>();
        var memoryCache = Substitute.For<IMemoryCache>();
        fileProvider.GetFileList("Pages").Returns(Array.Empty<string>());

        var provider = new PageDataProvider(fileProvider, memoryCache);

        var pages = await provider.GetPages();

        pages.Should().BeEmpty();
    }
}
