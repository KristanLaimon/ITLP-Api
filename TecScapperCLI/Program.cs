using TecScrapperLib;

var tecapi = new TecApi(disableLogging: false);
var res = await tecapi.Connect();
int algo = 3;




// const string url = "https://siia.lapaz.tecnm.mx/Login.aspx";
//
// var handler = new HttpClientHandler();
// handler.CookieContainer = new CookieContainer();
// var client = new HttpClient(handler);
// Console.WriteLine("[INFO]: Success | Created http client");
//
// //=========== GET (Initial cookies and hidden data to make the post) ==============
// HttpResponseMessage res = await client.GetAsync(url);
// if (!res.IsSuccessStatusCode)
// {
//     Console.WriteLine($"Tec API [Error]: Couldn't connect. StatusCode: ${res.StatusCode} Reason: {res.ReasonPhrase} (No internet? or service down!)");
//     Environment.Exit(1);
// }
//
// Console.WriteLine("Tec API [INFO]: Working | Starting to fetch initial cookie and asp.ned needed variables");
// CookieCollection cookies = handler.CookieContainer.GetCookies(new Uri(url));
// Cookie? REQ_aspNetCookie = cookies.FirstOrDefault(cookie => cookie.Name.Contains("ASP.NET"));
// if (REQ_aspNetCookie is null)
// {
//     Console.WriteLine("Tec API [Error]: Couldn't fetch correctly the cookie dependency. (Tec changed the name of the cookie to auth and log in...?)");
//     Environment.Exit(1 /*Error code*/);
// }
// Console.WriteLine("Tec API [Info]: Success | Fetched asp.net cookie");
//
//
// var parser = new HtmlParser();
// string htmlFrontPageText = await client.GetStringAsync(url);
// var document = parser.ParseDocument(htmlFrontPageText);
// IElement? REQ1__VIEWSTATE = document.QuerySelector("input[id=\"__VIEWSTATE\"][type=\"hidden\"]");
// IElement? REQ2__VIEWSTATEGENERATOR = document.QuerySelector("input[id=\"__VIEWSTATEGENERATOR\"][type=\"hidden\"]");
// IElement? REQ3__EVENTVALIDATION = document.QuerySelector("input[id=\"__EVENTVALIDATION\"][type=\"hidden\"]");
// if (REQ1__VIEWSTATE is null || REQ2__VIEWSTATEGENERATOR is null || REQ3__EVENTVALIDATION is null)
// {
//     Console.WriteLine("Tec API [Error]: Failed fetching asp.net hidden needed variables to log in");
//     Environment.Exit(1);
// }
//
// Console.WriteLine(((IHtmlInputElement)REQ1__VIEWSTATE).Value);
// Console.WriteLine(((IHtmlInputElement)REQ2__VIEWSTATEGENERATOR).Value);
// Console.WriteLine(((IHtmlInputElement)REQ3__EVENTVALIDATION).Value);
//
// Console.WriteLine("[INFO]: Success | Fetched asp.net post needed variables");
//
// {
//     "__VIEWSTATE": "/wEPDwUJMjk2OTE3ODE1D2QWAgIDD2QWAgIBD2QWAgIBD2QWAgIBDw8WAh4EVGV4dAUqTnVtZXJvIGRlIENvbnRyb2wgbyBjb250cmFzZcOxYSBpbmNvcnJlY3RhZGRk1OfGsKjRBcePpKNzsHiNaOFDrKCgNonJv/ifYhknr8g=",
//     "__VIEWSTATEGENERATOR": "C2EE9ABB",
//     "__EVENTVALIDATION": "/wEdAAS0k0h/4suh/RR3V1B8hefg9wJrVOvfPO1Uqu5toZDTTQozeEQ2Ny4O8RBKYR/qbE1MDg3KljjLOtYwCfQTyr/WBHZXKp/od6fzPo+46wAOggVpMxY1OobBU4p2SP3bPRw=",
//     "editControl": "d",
//     "editContraseña": "",
//     "btnEntrar": "Entrar"
// }




// var postBody = new FormUrlEncodedContent(new[]
// {
//     // new KeyValuePair<string, string>("__VIEWSTATE", viewState),
//     // new KeyValuePair<string, string>("__VIEWSTATEGENERATOR", viewStateGen),
//     // new KeyValuePair<string, string>("__EVENTVALIDATION", eventValidation),
//     new KeyValuePair<string, string>("editControl", "your-username"),
//     new KeyValuePair<string, string>("editContraseña", "your-password"),
//     new KeyValuePair<string, string>("btnEntrar", "Entrar")
// });
// var resPost = await client.PostAsync(url, postBody);
// Console.WriteLine("Finished");



var headers = new Dictionary<string, string>
{
    { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:139.0) Gecko/20100101 Firefox/139.0" },
    { "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8" },
    { "Accept-Language", "en-US,en;q=0.5" },
    { "Accept-Encoding", "gzip, deflate, br, zstd" },
    { "Content-Type", "application/x-www-form-urlencoded" }, // if POST request
    { "Origin", "https://siia.lapaz.tecnm.mx" },
    { "DNT", "1" },
    { "Sec-GPC", "1" },
    { "Referer", "https://siia.lapaz.tecnm.mx/Login.aspx" },
    { "Upgrade-Insecure-Requests", "1" },
    { "Sec-Fetch-Dest", "document" },
    { "Sec-Fetch-Mode", "navigate" },
    { "Sec-Fetch-Site", "same-origin" },
    { "Sec-Fetch-User", "?1" },
    { "Pragma", "no-cache" },
    { "Cache-Control", "no-cache" }
};
// Host: siia.lapaz.tecnm.mx
// User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:139.0) Gecko/20100101 Firefox/139.0
// Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
// Accept-Language: en-US,en;q=0.5
// Accept-Encoding: gzip, deflate, br, zstd
// Content-Type: application/x-www-form-urlencoded
// Content-Length: 442
// Origin: https://siia.lapaz.tecnm.mx
// DNT: 1
// Sec-GPC: 1
// Connection: keep-alive
// Referer: https://siia.lapaz.tecnm.mx/Login.aspx
// Cookie: ASP.NET_SessionId=n1pmopr432sr0xage1pgxbjt
// Upgrade-Insecure-Requests: 1
// Sec-Fetch-Dest: document
// Sec-Fetch-Mode: navigate
// Sec-Fetch-Site: same-origin
// Sec-Fetch-User: ?1
// Priority: u=0, i
// Pragma: no-cache
// Cache-Control: no-cache
// TE: trailers
