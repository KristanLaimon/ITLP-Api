using TecWrapperApi.Helpers;

namespace TecApiWrapperTesting.Helpers;

public class HeadersHelperTests
{
    private Dictionary<string, string> CustomHeaders = new() { { "MyCustomHeader", "valueForCustomHeader" } };
    
    [Fact]
    public void GetCommonHeaders_MustRetrieveCorrectlyDictionaryAppended()
    {
        var originalHeaders = HeadersHelper.GetCommonHeaders();
        var headersWithCustom = HeadersHelper.GetCommonHeaders(CustomHeaders);
        Assert.True((originalHeaders.Count + 1) == headersWithCustom.Count);
    }

    [Fact]
    public void GetCommonHeaders_AppendHeadersShouldNotModifyOriginalCommonHeaders()
    {
        var originalHeaders = HeadersHelper.GetCommonHeaders();
        var _ = HeadersHelper.GetCommonHeaders(CustomHeaders);
        var originalHeadersLater = HeadersHelper.GetCommonHeaders();
        Assert.True(originalHeaders.Count == originalHeadersLater.Count);
    }
}