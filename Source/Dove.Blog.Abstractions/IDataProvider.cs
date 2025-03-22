using System.Text;

namespace Dove.Blog.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IDataProvider
{
    /// <summary>
    /// 
    /// </summary>
    string RootPath { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageName"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    Task<string> ReadPageContent(string pageName, Encoding encoding = null!);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pageName"></param>
    /// <param name="content"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    Task WritePageContent(string pageName, string content, Encoding encoding = null!);
}
