namespace TecApiWrapperTesting;

public class MockPagesTest
{
    [Fact]
    public void MockPages_GetLoginPageHTML_ShouldBeFoundInDistFolder()
    {
        Assert.NotEmpty(MockPages.LoginPage);
        Assert.NotNull(MockPages.LoginPage);
    }

    [Fact]
    public void MockPages_GetMainPageHTML_ShouldBeFoundInDistFolder()
    {
        Assert.NotEmpty(MockPages.MainPage);
        Assert.NotNull(MockPages.MainPage); 
    }
}