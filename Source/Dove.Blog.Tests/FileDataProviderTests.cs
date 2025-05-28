using System.Text;
using Dove.Blog.Data;
using Microsoft.AspNetCore.Hosting;
using NSubstitute;
using Xunit;
using FluentAssertions;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace Dove.Blog.Tests;

public class FileDataProviderTests : IDisposable
{
    private readonly string _tempRoot;
    private readonly IHostingEnvironment _env;
    private readonly IFileSystem _fileSystem;
    private readonly FileDataProvider _provider;

    public FileDataProviderTests()
    {
        
        _tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempRoot);

        _env = Substitute.For<IHostingEnvironment>();
        _env.WebRootPath.Returns(_tempRoot);

        _fileSystem = new MockFileSystem();

        _provider = new FileDataProvider(_env, _fileSystem);
    }

    [Fact]
    public async Task GetFileList_ReturnsFilesWithoutExtension()
    {
        var dataDir = _fileSystem.Path.Combine(_tempRoot, "data", "pages");
        _fileSystem.Directory.CreateDirectory(dataDir);
        _fileSystem.File.WriteAllText(Path.Combine(dataDir, "test1.md"), "content1");
        _fileSystem.File.WriteAllText(Path.Combine(dataDir, "test2.md"), "content2");

        var files = await _provider.GetFileList("pages");

        files.Should().BeEquivalentTo(new[] { "test1", "test2" });
    }

    [Fact]
    public async Task GetFileList_ReturnsFilesWithExtension_WhenRequested()
    {
        var dataDir = _fileSystem.Path.Combine(_tempRoot, "data", "pages");
        _fileSystem.Directory.CreateDirectory(dataDir);
        _fileSystem.File.WriteAllText(Path.Combine(dataDir, "test1.md"), "content1");

        var files = await _provider.GetFileList("pages", withExtension: true);

        files.Should().ContainSingle(f => f == "test1.md");
    }

    [Fact]
    public async Task GetFileList_ReturnsEmpty_WhenDirectoryDoesNotExist()
    {
        var files = await _provider.GetFileList("doesnotexist");
        files.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadPageContent_ReturnsContent_WhenFileExists()
    {
        var dataDir = _fileSystem.Path.Combine(_tempRoot, "data");
        _fileSystem.Directory.CreateDirectory(dataDir);
        var filePath = _fileSystem.Path.Combine(dataDir, "mypage.md");
        var expected = "Hello, world!";
        await _fileSystem.File.WriteAllTextAsync(filePath, expected, Encoding.UTF8);

        var content = await _provider.ReadPageContent("mypage");

        content.Should().Be(expected);
    }

    [Fact]
    public async Task ReadPageContent_Throws_WhenFileDoesNotExist()
    {
        Func<Task> act = async () => await _provider.ReadPageContent("missingpage");
        await act.Should().ThrowAsync<FileNotFoundException>();
    }

    [Fact]
    public async Task WritePageContent_CreatesFileWithContent()
    {
        var dataDir = _fileSystem.Path.Combine(_tempRoot, "data");
        _fileSystem.Directory.CreateDirectory(dataDir);

        await _provider.WritePageContent("newpage", "test content");

        var filePath = _fileSystem.Path.Combine(dataDir, "newpage.md");
        _fileSystem.File.Exists(filePath).Should().BeTrue();
        var content = await _fileSystem.File.ReadAllTextAsync(filePath);
        content.Should().Be("test content");
    }

    public void Dispose()
    {
        if (_fileSystem.Directory.Exists(_tempRoot))
            _fileSystem.Directory.Delete(_tempRoot, true);
    }
}
