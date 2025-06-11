using System.Net;
using TecScrapperLib.Types;

namespace TecScrapperLib.Factory;
internal static class TecApiStatusGenerator
{
    public static TecApiStatus GenerateOK()
    {
        return new TecApiStatus
        {
            StatusCode = HttpStatusCode.OK,
            Reason = "OK",
            Details = "All good"
        };
    }
}
