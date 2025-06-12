using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace TecWrapperApi.Helpers;

internal static class HtmlPageHelper
{
    private static HtmlParser parser = new HtmlParser();
    private static string? cachedTextDocument;
    private static IHtmlDocument? cachedDocument;
    
    internal static bool IsLoginPage(string htmlPage)
    {
        if (string.IsNullOrEmpty(htmlPage)) return false;
        
        parser = new HtmlParser();
        var document = ParseTextDocument(htmlPage);
        IHtmlInputElement? inputButton = document.QuerySelector<IHtmlInputElement>("input[id=\"editControl\"]");
        return inputButton != null;
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

    /// <summary>
    /// Find all elements in html that fulfills the given cssSelector
    /// </summary>
    /// <param name="htmlPageContent">The HTML content to search within</param>
    /// <param name="cssSelector">The css selector to match the element</param>
    /// <typeparam name="T">The type of the element to return, which must implement <see cref="IElement"/> and be a class as well (not a interface only)</typeparam>
    /// <returns>A collection (list) of elements that matced the cssSelector, if not found, returns an empty array. (can't return null)</returns>
    public static T[] FindMany<T>(string htmlPageContent, string cssSelector) where T : class, IElement
    {
        if (string.IsNullOrEmpty(htmlPageContent)) return [];
        var document = ParseTextDocument(htmlPageContent);
        var found = document.QuerySelectorAll(cssSelector);
        return found as T[] ?? [];
    }
    
    private static IHtmlDocument ParseTextDocument(string htmlPage)
    {
        if (cachedTextDocument is null || cachedTextDocument == htmlPage)
            return cachedDocument ?? parser.ParseDocument(htmlPage);
        
        cachedTextDocument = htmlPage;
        cachedDocument = parser.ParseDocument(htmlPage);

        return cachedDocument;
    }
}