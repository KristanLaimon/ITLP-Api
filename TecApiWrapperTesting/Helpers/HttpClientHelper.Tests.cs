using TecWrapperApi.Helpers;

namespace TecApiWrapperTesting.Helpers;

public class HttpClientHelperTests
{
   
   [Fact]
   public async Task WithHeadersContext_MustInjectHeadersOnlyWhileInCallback()
   {
      var headersToInject = HeadersHelper.GetCommonHeaders();
      var outerClient = new HttpClient();
      Assert.Empty(outerClient.DefaultRequestHeaders);
      await HttpClientHelper.WithHeadersContext(outerClient, headersToInject, (innerClient) =>
      {
         Assert.True(innerClient.DefaultRequestHeaders.Count() == headersToInject.Count);
         Assert.True(outerClient.DefaultRequestHeaders.Count() == headersToInject.Count);
         return Task.CompletedTask;
      });
      Assert.Empty(outerClient.DefaultRequestHeaders);
   }
}