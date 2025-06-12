using System.Net;

namespace TecWrapperApi.Types;

public record TecApiStatus
{
    public required HttpStatusCode StatusCode;
    public required string Reason;
    public required string Details;
    public bool WasSuccessful() => this.StatusCode == HttpStatusCode.OK;
}
