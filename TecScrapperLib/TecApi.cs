using System.Net;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using TecScrapperLib.__internals__;
using TecScrapperLib.Factory;
using TecScrapperLib.Types;
using TecScrapperLib.Utils;

namespace TecScrapperLib;

public class TecApi
{
    private readonly string URL = "https://siia.lapaz.tecnm.mx/Login.aspx";
    private readonly Logger logger;
    private readonly HttpClientHandler httpHandler;
    private readonly HttpClient httpClient;
    
    public TecApi(string? customUrl = null, bool disableLogging = true)
    {
        this.URL = customUrl ?? this.URL;
        this.httpHandler = new HttpClientHandler();
        this.httpHandler.CookieContainer = new CookieContainer();
        this.httpClient = new HttpClient(this.httpHandler);
        this.logger = new Logger(disable: disableLogging);
        this.logger.Log(LogLevel.Info, "Created http client");
    }
    
    public async Task<TecApiStatus> Connect()
    {
        var initialFetchRes = await this.FetchInitialGet();
        if (!initialFetchRes.httpStatus.WasSuccessful()) return initialFetchRes.httpStatus;
        
        var fetchingCookieRes = await this.FetchCookie();
        if (!fetchingCookieRes.httpStatus.WasSuccessful()) return fetchingCookieRes.httpStatus;
        Cookie depCookie = (Cookie)fetchingCookieRes.somethingToReturn!;
        
        var fetchingVariablesForPost = await this.FetchRequiredPostValues();
        if (!fetchingVariablesForPost.httpStatus.WasSuccessful()) return fetchingVariablesForPost.httpStatus;
        Dictionary<string, string> postVariablesToSend = (Dictionary<string,string>)fetchingVariablesForPost.somethingToReturn!;

        List<KeyValuePair<string, string>> list = new()
        {
            new KeyValuePair<string, string>("editControl", "noControlHERE"),
            new KeyValuePair<string, string>("editContraseña", "passwordHERE"), //TODO: Not completed, just for testing
            new KeyValuePair<string, string>("btnEntrar", "Entrar")
        };
        list.AddRange(postVariablesToSend.Select(key_Value => new KeyValuePair<string, string>(key_Value.Key, key_Value.Value)));
        
        var postBody = new FormUrlEncodedContent(list.ToArray());
        var postHeaders = HeadersHelper.GetCommonHeaders(anyExtraHeadersToUse: [new KeyValuePair<string, string>("Cookie",  $"ASP.NET_SessionId={depCookie.Value}")]);
        
        string mainPageInfoHtmlText;
        await HttpClientHelper.WithHeadersContext(this.httpClient, postHeaders, async (httpClientWithHeaders) =>
        {
            
            var res = await httpClient.PostAsync("https://siia.lapaz.tecnm.mx/Login.aspx", postBody); 
            mainPageInfoHtmlText = await res.Content.ReadAsStringAsync();
            // if (htmlPage.Contains("Instituto Tecnologico de La Paz") && htmlPage.Contains("No. de Control:"))
        });
        
        return await Task.FromResult(TecApiStatusGenerator.GenerateOK());
    }

    private async Task<TecApiConnectionStepStatus> FetchInitialGet()
    {
        HttpResponseMessage res = await this.httpClient.GetAsync(this.URL);
        if (res.IsSuccessStatusCode)
            return new TecApiConnectionStepStatus { httpStatus = TecApiStatusGenerator.GenerateOK() };
        
        this.logger.Log(LogLevel.Error, $"Couldn't connect. StatusCode: ${res.StatusCode} Reason: {res.ReasonPhrase} (No internet? or service down!)");
        var toReturn =  new TecApiStatus
        {
            StatusCode = res.StatusCode,
            Reason = res.ReasonPhrase ?? "No reason...",
            Details = $"Couldn't connect. StatusCode: ${res.StatusCode} Reason: {res.ReasonPhrase} (No internet? or service down!)"
        };
        return new TecApiConnectionStepStatus { httpStatus = toReturn };
    }

    private Task<TecApiConnectionStepStatus> FetchCookie()
    {
        this.logger.Log(LogLevel.Info, "Starting to fetch initial cookie and asp.ned needed variables");
        CookieCollection cookies = this.httpHandler.CookieContainer.GetCookies(new Uri(this.URL));
        Cookie? REQ_aspNetCookie = cookies.FirstOrDefault(cookie => cookie.Name.Contains("ASP.NET"));
        if (REQ_aspNetCookie is null)
        {
            this.logger.Log(LogLevel.Error, "Couldn't fetch correctly the cookie dependency. (Tec changed the name of the cookie to auth and log in...?)");
            var firstToReturn = new TecApiStatus
            {
                StatusCode = HttpStatusCode.ServiceUnavailable,
                Reason =
                    "Couldn't fetch the cookie, maybe the service is down or they change the way the authenticate users log-in",
                Details = $"Couldn't fetch correctly the cookie dependency."
            };
            var secondToReturn =  new TecApiConnectionStepStatus { httpStatus = firstToReturn };
            return Task.FromResult(secondToReturn);
        }
        this.logger.Log(LogLevel.Info, "Fetched tec asp.net cookie dependency");
        var toReturn = new TecApiConnectionStepStatus
            { httpStatus = TecApiStatusGenerator.GenerateOK(), somethingToReturn = REQ_aspNetCookie };
        return Task.FromResult(toReturn);
    }

    private async Task<TecApiConnectionStepStatus> FetchRequiredPostValues()
    {
        var parser = new HtmlParser();
        string htmlFrontPageText = await this.httpClient.GetStringAsync(this.URL);
        var document = parser.ParseDocument(htmlFrontPageText);
        IHtmlInputElement? REQ1__VIEWSTATE = (IHtmlInputElement)document.QuerySelector("input[id=\"__VIEWSTATE\"][type=\"hidden\"]")!;
        IHtmlInputElement? REQ2__VIEWSTATEGENERATOR = (IHtmlInputElement)document.QuerySelector("input[id=\"__VIEWSTATEGENERATOR\"][type=\"hidden\"]")!;
        IHtmlInputElement? REQ3__EVENTVALIDATION = (IHtmlInputElement)document.QuerySelector("input[id=\"__EVENTVALIDATION\"][type=\"hidden\"]")!;
        IHtmlInputElement?[] allValuesNeccesary = new []{REQ1__VIEWSTATE, REQ2__VIEWSTATEGENERATOR, REQ3__EVENTVALIDATION};
        if (allValuesNeccesary.Any(inputElement => inputElement is null))
        {
            var toReturnFirst = new TecApiStatus
            {
                StatusCode = HttpStatusCode.ServiceUnavailable,
                Reason = "Coudn't get all the asp.net values hidden on index hidden inputs..., maybe they change de ui layout (?)",
                Details = $"From 3 expected values: __VIEWSTATE, __VIEWSTATEGENERATOR, __EVENTVALIDATION, only got: {allValuesNeccesary.Where(inputElement => inputElement is not null).Select(inputElement => inputElement!.Id)}"
            };
            var toReturnSecond = new TecApiConnectionStepStatus { httpStatus = toReturnFirst };
            return toReturnSecond;
        }

        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        foreach (var element in allValuesNeccesary) 
            dictionary.Add(element!.Id!, element!.Value);
        return new TecApiConnectionStepStatus
        {
            httpStatus = TecApiStatusGenerator.GenerateOK(),
            somethingToReturn = dictionary
        };
    }
}