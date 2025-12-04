namespace Loom.Application.Services;

public sealed class ApiError
{
    public string Message { get; }

    public ApiError(string message)
    {
        Message = message;
    }

    public override string ToString() => Message;
}
