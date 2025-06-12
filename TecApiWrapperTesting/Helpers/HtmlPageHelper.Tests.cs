using AngleSharp.Dom;
using TecWrapperApi.Helpers;
namespace TecApiWrapperTesting.Helpers;


public class HtmlPageHelperTests
{
    private readonly string LoginPage = MockPages.LoginPage;
    private readonly string MainPage = MockPages.MainPage;
    
    [Fact]
    public void IsLoginPage_MustReturnTrueWithLoginPage()
    {
        bool found = HtmlPageHelper.IsLoginPage(this.LoginPage);
        Assert.True(found);
    }

    [Fact]
    public void IsLoginPage_MustReturnFalseIfNotLoginPage()
    {
        bool found = HtmlPageHelper.IsLoginPage(this.MainPage);
        Assert.False(found);
    }

    [Theory]
    [InlineData("asdf", "asdf", " asdf" )]
    public void FindFirst_MustFindCorrectlyHtmlElement(string htmlContent, string cssSelector, string expectedElementType)
    {
        
    }
}