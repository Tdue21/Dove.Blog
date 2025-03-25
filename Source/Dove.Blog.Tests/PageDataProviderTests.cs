using Dove.Blog.Abstractions;
using Dove.Blog.Logic;
using FluentAssertions;
using NSubstitute;

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
}
