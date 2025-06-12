using System.Net;
using TecWrapperApi.Types;

namespace TecWrapperApi.Factory;
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
