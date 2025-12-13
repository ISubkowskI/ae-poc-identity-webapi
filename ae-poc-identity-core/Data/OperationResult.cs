namespace Ae.Poc.Identity.Data;

public sealed record OperationResult<T>
{
    public bool IsSuccess { get; set; } = false;
    public string InfoMessage { get; set; } = string.Empty;
    public T? Value { get; set; } = default;

    public static OperationResult<T> Success(T value, string message = "") =>
        new()
        { IsSuccess = true, Value = value, InfoMessage = message };

    public static OperationResult<T> Failure(string message) =>
        new()
        { IsSuccess = false, InfoMessage = message };
}
