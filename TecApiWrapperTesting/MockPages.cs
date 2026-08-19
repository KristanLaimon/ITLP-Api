namespace TecApiWrapperTesting;

public static class MockPages
{
    public static string LoginPage => File.ReadAllText("./MockPagesFiles/LoginPage.html");
    public static string LoginFailedPage => File.ReadAllText("./MockPagesFiles/LoginFailedPage.html");
    public static string StudentStatusPage => File.ReadAllText("./MockPagesFiles/StudentStatusPage.html");
    public static string GradeHistoryPage => File.ReadAllText("./MockPagesFiles/GradeHistoryPage.html");
}
