namespace TecWrapperApi.Helpers;

/// <summary>
/// Provides utility methods for managing HTTP client headers.
/// </summary>
internal static class HttpClientHelper
{
    /// <summary>
    /// Executes an action on an <see cref="HttpClient"/> instance within a controlled header context.
    /// This method temporarily applies the specified headers, executes the provided action, and then clears the headers.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> instance to configure and use.</param>
    /// <param name="headersToUse">A dictionary containing header names and their corresponding values to be applied to the client.</param>
    /// <param name="functToExecute">The action to execute on the <see cref="HttpClient"/> with the configured headers.</param>
    /// <remarks>
    /// This method ensures that the <see cref="HttpClient.DefaultRequestHeaders"/> are cleared before and after execution to prevent header leakage between requests.
    /// <para>Examples:</para>
    /// <code>
    /// // Example 1: Making a GET request with custom headers
    /// var client = new HttpClient();
    /// var headers = new Dictionary&lt;string, string&gt;
    /// {
    ///     { "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8" },
    ///     { "User-Agent", "MyCustomAgent/1.0" }
    /// };
    /// HeadersHelper.ExecuteHttpClientWithHeadersContext(client, headers, async (httpClient) =>
    /// {
    ///     var response = await httpClient.GetAsync("https://example.com");
    ///     var content = await response.Content.ReadAsStringAsync();
    ///     Console.WriteLine(content);
    /// });
    ///
    /// // Example 2: Making a POST request with headers
    /// var postClient = new HttpClient();
    /// var postHeaders = new Dictionary&lt;string, string&gt;
    /// {
    ///     { "Accept", "application/json" },
    ///     { "Content-Type", "application/json" }
    /// };
    /// var postData = new StringContent("{ \"key\": \"value\" }", Encoding.UTF8, "application/json");
    /// HeadersHelper.ExecuteHttpClientWithHeadersContext(postClient, postHeaders, async (httpClient) =>
    /// {
    ///     var response = await httpClient.PostAsync("https://api.example.com/data", postData);
    ///     var result = await response.Content.ReadAsStringAsync();
    ///     Console.WriteLine(result);
    /// });
    /// </code>
    /// </remarks>
    public static async Task WithHeadersContext(HttpClient client, Dictionary<string, string> headersToUse, Func<HttpClient, Task> functToExecute)
    {
        client.DefaultRequestHeaders.Clear();
        foreach (var header_value in headersToUse)
            client.DefaultRequestHeaders.Add(header_value.Key, header_value.Value);
        await functToExecute(client);
        client.DefaultRequestHeaders.Clear();
    }
}