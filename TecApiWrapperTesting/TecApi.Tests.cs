using System.Net;
using TecWrapperApi;
using TecWrapperApi.Exceptions;

namespace TecApiWrapperTesting;

public class TecApiTests
{
    private static HttpResponseMessage Html(string content) => new(HttpStatusCode.OK)
    {
        Content = new StringContent(content),
    };

    private static FakeHttpMessageHandler NeverCalledHandler() =>
        new(_ => throw new InvalidOperationException("Shouldn't make any request before login"));

    [Fact]
    public async Task LoginAsync_WithValidCredentials_MarksApiAsLoggedIn()
    {
        var handler = new FakeHttpMessageHandler(request =>
        {
            string path = request.RequestUri!.AbsolutePath;
            if (request.Method == HttpMethod.Get && path == "/login")
                return Html(MockPages.LoginPage);
            if (request.Method == HttpMethod.Post && path == "/login")
                return Html(MockPages.StudentStatusPage);
            throw new InvalidOperationException($"Unexpected request: {request.Method} {path}");
        });
        var api = new TecApi("00000000", "correct-password", httpMessageHandler: handler);

        await api.LoginAsync();

        Assert.True(api.IsLoggedIn);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ThrowsTecApiInvalidCredentialsException()
    {
        var handler = new FakeHttpMessageHandler(request =>
        {
            string path = request.RequestUri!.AbsolutePath;
            if (request.Method == HttpMethod.Get && path == "/login")
                return Html(MockPages.LoginPage);
            if (request.Method == HttpMethod.Post && path == "/login")
                return Html(MockPages.LoginFailedPage);
            throw new InvalidOperationException($"Unexpected request: {request.Method} {path}");
        });
        var api = new TecApi("00000000", "wrong-password", httpMessageHandler: handler);

        var ex = await Assert.ThrowsAsync<TecApiInvalidCredentialsException>(api.LoginAsync);
        Assert.Contains("incorrectos", ex.Message);
        Assert.False(api.IsLoggedIn);
    }

    [Fact]
    public async Task LoginAsync_WhenLoginPageHasNoCsrfToken_ThrowsTecApiParsingException()
    {
        var handler = new FakeHttpMessageHandler(_ => Html("<html><body>no form here</body></html>"));
        var api = new TecApi("00000000", "any", httpMessageHandler: handler);

        await Assert.ThrowsAsync<TecApiParsingException>(api.LoginAsync);
    }

    [Fact]
    public async Task LoginAsync_WhenSiteUnreachable_ThrowsTecApiConnectionException()
    {
        var handler = new FakeHttpMessageHandler(_ => throw new HttpRequestException("boom"));
        var api = new TecApi("00000000", "any", httpMessageHandler: handler);

        await Assert.ThrowsAsync<TecApiConnectionException>(api.LoginAsync);
    }

    [Fact]
    public async Task GetStudentStatusAsync_WithoutLoggingIn_ThrowsTecApiNotLoggedInException()
    {
        var api = new TecApi("00000000", "any", httpMessageHandler: NeverCalledHandler());

        await Assert.ThrowsAsync<TecApiNotLoggedInException>(api.GetStudentStatusAsync);
    }

    [Fact]
    public async Task GetGradeHistoryAsync_WithoutLoggingIn_ThrowsTecApiNotLoggedInException()
    {
        var api = new TecApi("00000000", "any", httpMessageHandler: NeverCalledHandler());

        await Assert.ThrowsAsync<TecApiNotLoggedInException>(api.GetGradeHistoryAsync);
    }

    [Fact]
    public async Task GetStudentStatusAsync_AfterLogin_ReturnsParsedStatus()
    {
        var handler = new FakeHttpMessageHandler(request =>
        {
            string path = request.RequestUri!.AbsolutePath;
            if (request.Method == HttpMethod.Get && path == "/login")
                return Html(MockPages.LoginPage);
            if (request.Method == HttpMethod.Post && path == "/login")
                return Html(MockPages.StudentStatusPage);
            if (request.Method == HttpMethod.Get && path == "/alumnos")
                return Html(MockPages.StudentStatusPage);
            throw new InvalidOperationException($"Unexpected request: {request.Method} {path}");
        });
        var api = new TecApi("00000000", "correct-password", httpMessageHandler: handler);
        await api.LoginAsync();

        var status = await api.GetStudentStatusAsync();

        Assert.Equal("00000000", status.Control);
        Assert.Equal("JUAN PEREZ GOMEZ", status.Nombre);
        Assert.Equal(9, status.Semestre);
    }

    [Fact]
    public async Task GetGradeHistoryAsync_AfterLogin_ReturnsParsedSemesters()
    {
        var handler = new FakeHttpMessageHandler(request =>
        {
            string path = request.RequestUri!.AbsolutePath;
            if (request.Method == HttpMethod.Get && path == "/login")
                return Html(MockPages.LoginPage);
            if (request.Method == HttpMethod.Post && path == "/login")
                return Html(MockPages.StudentStatusPage);
            if (request.Method == HttpMethod.Get && path == "/alumnos/historial-academico")
                return Html(MockPages.GradeHistoryPage);
            throw new InvalidOperationException($"Unexpected request: {request.Method} {path}");
        });
        var api = new TecApi("00000000", "correct-password", httpMessageHandler: handler);
        await api.LoginAsync();

        var semesters = await api.GetGradeHistoryAsync();

        Assert.Equal(8, semesters.Count);
        Assert.Equal(1, semesters[0].Numero);
    }
}
