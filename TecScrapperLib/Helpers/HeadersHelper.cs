using System.Runtime.CompilerServices;

namespace TecScrapperLib.__internals__;

internal static class HeadersHelper
{
    private static readonly Dictionary<string,string> CommonHeadersDict = new Dictionary<string, string>
    {
        { "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8" },
        { "Accept-Encoding", "gzip, deflate, br, zstd" },
        { "Accept-Language", "en-US,en;q=0.5" },
        { "Cache-Control", "no-cache" },
        { "Connection", "keep-alive" },
        { "Pragma", "no-cache" },
        { "Priority", "u=0, i" },
        { "Referer", "https://siia.lapaz.tecnm.mx/Login.aspx" },
        { "Sec-Fetch-Dest", "document" },
        { "Sec-Fetch-Mode", "navigate" },
        { "Sec-Fetch-Site", "same-origin" },
        { "Sec-Fetch-User", "?1" },
        { "Sec-GPC", "1" },
        { "TE", "trailers" },
        { "Upgrade-Insecure-Requests", "1" },
        { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:139.0) Gecko/20100101 Firefox/139.0" }
    };

    public static Dictionary<string, string> GetCommonHeaders(IEnumerable<KeyValuePair<string, string>>? anyExtraHeadersToUse = null)
    {
        if (anyExtraHeadersToUse is null)
            return CommonHeadersDict;
        else
        {
            var copy = new Dictionary<string, string>(CommonHeadersDict);
            foreach (var keyValuePair in anyExtraHeadersToUse)
                copy[keyValuePair.Key] = keyValuePair.Value;
            return copy;
        }
    }
}