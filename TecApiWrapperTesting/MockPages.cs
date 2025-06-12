namespace TecApiWrapperTesting;

public static class MockPages
{
    public static string LoginPage => File.ReadAllText("./MockPagesFiles/Login.html");
    public static string MainPage => File.ReadAllText("./MockPagesFiles/Main.html");
}