namespace TecApiWrapperTesting;

public class MockPagesTest
{
    [Fact]
    public void MockPages_GetLoginPageHTML_ShouldBeFoundInDistFolder()
    {
        Assert.NotEmpty(MockPages.LoginPage);
    }

    [Fact]
    public void MockPages_GetLoginFailedPageHTML_ShouldBeFoundInDistFolder()
    {
        Assert.NotEmpty(MockPages.LoginFailedPage);
    }

    [Fact]
    public void MockPages_GetStudentStatusPageHTML_ShouldBeFoundInDistFolder()
    {
        Assert.NotEmpty(MockPages.StudentStatusPage);
    }

    [Fact]
    public void MockPages_GetGradeHistoryPageHTML_ShouldBeFoundInDistFolder()
    {
        Assert.NotEmpty(MockPages.GradeHistoryPage);
    }
}
