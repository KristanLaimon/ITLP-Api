using System.Net;
using System.Text.RegularExpressions;
using AngleSharp.Html.Dom;
using TecWrapperApi.Exceptions;
using TecWrapperApi.Helpers;
using TecWrapperApi.Types;
using TecWrapperApi.Utils;

namespace TecWrapperApi;

public partial class TecApi
{
    private const string BaseUrl = "https://siia2.lapaz.tecnm.mx";
    private const string LoginUrl = $"{BaseUrl}/login";
    private const string StudentStatusUrl = $"{BaseUrl}/alumnos";
    private const string GradeHistoryUrl = $"{BaseUrl}/alumnos/historial-academico";

    [GeneratedRegex("data-errors='(\\[[^']*])'")]
    private static partial Regex LoginErrorsRegex();

    private readonly Logger logger;
    private readonly HttpClientHandler httpHandler;
    private readonly HttpClient httpClient;
    private readonly string noControl;
    private readonly string password;

    public bool IsLoggedIn { get; private set; }

    public TecApi(
        string noControl,
        string password,
        bool disableLogging = true,
        HttpMessageHandler? httpMessageHandler = null
    )
    {
        this.noControl = noControl;
        this.password = password;
        this.httpHandler = new HttpClientHandler { CookieContainer = new CookieContainer() };
        this.httpClient = new HttpClient(httpMessageHandler ?? this.httpHandler);
        this.logger = new Logger(disable: disableLogging);
        this.logger.Log(LogLevel.Info, "Created http client");
    }

    /// <summary>
    /// Authenticates against siia2 using the credentials given at construction time and keeps the
    /// resulting session cookies in memory, so <see cref="GetStudentStatusAsync"/> and
    /// <see cref="GetGradeHistoryAsync"/> can be called afterward without logging in again.
    /// </summary>
    public async Task LoginAsync()
    {
        string loginPageHtml = await this.FetchHtmlOrThrow(LoginUrl);
        string csrfToken = ExtractCsrfTokenOrThrow(loginPageHtml);
        var formValues = HeadersHelper.GetAuthFormValues(csrfToken, this.noControl, this.password);

        HttpResponseMessage response;
        try
        {
            response = await this.httpClient.PostAsync(LoginUrl, new FormUrlEncodedContent(formValues));
        }
        catch (HttpRequestException ex)
        {
            throw new TecApiConnectionException(
                "Couldn't reach the login endpoint. (No internet? or service down?)",
                ex
            );
        }

        string resultHtml = await response.Content.ReadAsStringAsync();
        ThrowIfLoginFailed(resultHtml);

        this.IsLoggedIn = true;
        this.logger.Log(LogLevel.Info, "Logged in successfully");
    }

    /// <summary>Fetches the student's general status/kardex summary. Requires a prior <see cref="LoginAsync"/>.</summary>
    public async Task<StudentStatus> GetStudentStatusAsync()
    {
        this.EnsureLoggedIn();
        string html = await this.FetchHtmlOrThrow(StudentStatusUrl);
        return StudentStatusParser.Parse(html);
    }

    /// <summary>Fetches the full per-semester grade history. Requires a prior <see cref="LoginAsync"/>.</summary>
    public async Task<List<SemesterHistory>> GetGradeHistoryAsync()
    {
        this.EnsureLoggedIn();
        string html = await this.FetchHtmlOrThrow(GradeHistoryUrl);
        return GradeHistoryParser.Parse(html);
    }

    private void EnsureLoggedIn()
    {
        if (!this.IsLoggedIn)
            throw new TecApiNotLoggedInException("Call LoginAsync() before requesting student data.");
    }

    private async Task<string> FetchHtmlOrThrow(string url)
    {
        HttpResponseMessage response;
        try
        {
            response = await this.httpClient.GetAsync(url);
        }
        catch (HttpRequestException ex)
        {
            throw new TecApiConnectionException(
                $"Couldn't connect to {url}. (No internet? or service down?)",
                ex
            );
        }

        if (!response.IsSuccessStatusCode)
        {
            this.logger.Log(
                LogLevel.Error,
                $"Couldn't fetch {url}. StatusCode: {response.StatusCode} Reason: {response.ReasonPhrase}"
            );
            throw new TecApiConnectionException(
                $"{url} responded with {(int)response.StatusCode} {response.ReasonPhrase}"
            );
        }

        return await response.Content.ReadAsStringAsync();
    }

    private static string ExtractCsrfTokenOrThrow(string loginPageHtml)
    {
        var tokenInput = HtmlPageHelper.FindFirst<IHtmlInputElement>(loginPageHtml, "input[name=\"_token\"]");
        if (tokenInput is null || string.IsNullOrEmpty(tokenInput.Value))
            throw new TecApiParsingException(
                "Couldn't find the CSRF token on the login page. (Tec changed the login form layout?)"
            );
        return tokenInput.Value;
    }

    private static void ThrowIfLoginFailed(string resultHtml)
    {
        if (!HtmlPageHelper.IsLoginPage(resultHtml))
            return;

        var errorsMatch = LoginErrorsRegex().Match(resultHtml);
        string reason = errorsMatch.Success ? errorsMatch.Groups[1].Value : "Usuario o contraseña incorrectos";
        throw new TecApiInvalidCredentialsException(reason);
    }
}
