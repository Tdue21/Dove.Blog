using Dove.Blog.Abstractions;
using Dove.Blog.Logic;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Dove.Blog.Tests;

public class PageDataProviderTests
{
    [Fact]
    public async Task GetPage_Success_TestAsync()
    {
        var fileProvider = Substitute.For<IDataProvider>();
        fileProvider.ReadPageContent(Arg.Any<string>()).Returns("## Hello, World!");
        var provider = new PageDataProvider(fileProvider);
        var page = await provider.GetPage("Index");

        page.Should().NotBeNull();
        page.Content.Should().NotBeNullOrEmpty();
        page.Content.Should().BeEquivalentTo("<h2>Hello, World!</h2>\n");
    }

    [Fact]
    public async Task GetPage_Page_Not_Found_TestAsync()
    {
        var fileProvider = Substitute.For<IDataProvider>();
        fileProvider.ReadPageContent(Arg.Any<string>()).Throws<FileNotFoundException>();

        var provider = new PageDataProvider(fileProvider);

        await provider.Invoking(x => x.GetPage("Index")).Should().ThrowAsync<FileNotFoundException>();
    }
}
