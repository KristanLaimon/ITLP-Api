namespace TecWrapperApi.Exceptions;

public class TecApiException : Exception
{
    public TecApiException(string message) : base(message) { }

    public TecApiException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>Thrown when the site can't be reached or responds with a non-success status.</summary>
public class TecApiConnectionException : TecApiException
{
    public TecApiConnectionException(string message) : base(message) { }

    public TecApiConnectionException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>Thrown when the login form rejects the given noControl/password.</summary>
public class TecApiInvalidCredentialsException : TecApiException
{
    public TecApiInvalidCredentialsException(string message) : base(message) { }
}

/// <summary>Thrown when a data method is called before a successful LoginAsync().</summary>
public class TecApiNotLoggedInException : TecApiException
{
    public TecApiNotLoggedInException(string message) : base(message) { }
}

/// <summary>Thrown when an expected page element/field is missing, likely because Tec changed the page layout.</summary>
public class TecApiParsingException : TecApiException
{
    public TecApiParsingException(string message) : base(message) { }
}
