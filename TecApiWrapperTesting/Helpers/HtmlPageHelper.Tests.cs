using TecWrapperApi.Helpers;
namespace TecApiWrapperTesting.Helpers;


public class HtmlPageHelperTests
{
    private readonly string LoginPage = MockPages.LoginPage;
    private readonly string StudentStatusPage = MockPages.StudentStatusPage;

    [Fact]
    public void IsLoginPage_MustReturnTrueWithLoginPage()
    {
        bool found = HtmlPageHelper.IsLoginPage(this.LoginPage);
        Assert.True(found);
    }

    [Fact]
    public void IsLoginPage_MustReturnFalseIfNotLoginPage()
    {
        bool found = HtmlPageHelper.IsLoginPage(this.StudentStatusPage);
        Assert.False(found);
    }
}