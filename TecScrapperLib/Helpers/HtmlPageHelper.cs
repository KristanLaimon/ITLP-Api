using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace TecScrapperLib.Helpers;

internal static class HtmlPageHelper
{
    private static HtmlParser parser = new HtmlParser();
    private static string? cachedTextDocument;
    private static IHtmlDocument? cachedDocument;
    
    public static bool IsLoginPage(string htmlPage)
    {
        if (string.IsNullOrEmpty(htmlPage)) return false;
        
        parser = new HtmlParser();
        var document = parser.ParseDocument(htmlPage);
        // IElement? inputButton = 
        return false;
    }

    private static IHtmlDocument ParseTextDocument(string htmlPage)
    {
        if (cachedTextDocument is not null && cachedTextDocument != htmlPage)
        {
            cachedTextDocument = htmlPage;
            cachedDocument = parser.ParseDocument(htmlPage);
        }
        
        return cachedDocument ?? parser.ParseDocument(htmlPage);
    }
    
    /// <summary>
    /// Finds the first element in the HTML content that matches the specified CSS selector.
    /// </summary>
    /// <typeparam name="T">The type of element to return, which must implement <see cref="IElement"/> and be a reference type.</typeparam>
    /// <param name="htmlPageContent">The HTML content to search within.</param>
    /// <param name="cssSelector">The CSS selector to match the element.</param>
    /// <returns>The first element matching the CSS selector, cast to type <typeparamref name="T"/>, or <c>null</c> if no match is found or the input is null/empty.</returns>
    /// <remarks>
    /// The method uses AngleSharp to parse the HTML and query for the first matching element. The result is cast to the specified type <typeparamref name="T"/>. If the cast fails or no element is found, <c>null</c> is returned.
    /// <para>Example:</para>
    /// <code>
    /// var input = HtmlPageHelper.FindFirst/<IHtmlInputElement/>(html, "input#editControl");
    /// if (input != null) Console.WriteLine(input.Id);
    /// </code>
    /// </remarks>
    public static T? FindFirst<T>(string htmlPageContent, string cssSelector) where T : class, IElement
    {
        if (string.IsNullOrEmpty(htmlPageContent)) return null;
        var document = ParseTextDocument(htmlPageContent);
        var found = document.QuerySelector(cssSelector);
        return found as T;
    }

    public static T[]? FindMany<T>(string htmlPageContent, string cssSelector) where T : class, IElement
    {
        if (string.IsNullOrEmpty(htmlPageContent)) return null;
        var document = ParseTextDocument(htmlPageContent);
        return [];
    }
}