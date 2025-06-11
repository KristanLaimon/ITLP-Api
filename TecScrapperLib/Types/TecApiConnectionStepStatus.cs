namespace TecScrapperLib.Types;

public record TecApiConnectionStepStatus
{
    public required TecApiStatus httpStatus;
    public object? somethingToReturn;
}