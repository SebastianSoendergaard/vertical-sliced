namespace Framework;

public sealed record Error(string Code, string Description);

public class Result<TSuccess>
{
    private TSuccess? _success{ get; }
    private Error? _error { get; }

    protected Result(TSuccess success) 
    { 
        _success = success;
    }

    protected Result(Error error)
    {
        _error = error;
    }

    public T Match<T>(Func<TSuccess?, T> onSuccess, Func<Error?, T> onFailure)
    {
        if (onSuccess == null)
        {
            throw new ArgumentNullException(nameof(onSuccess));
        }

        if (onFailure == null)
        {
            throw new ArgumentNullException(nameof(onFailure));
        }

        return _error == null ? onSuccess(_success) : onFailure(_error);
    }

    public bool IsSuccess { get { return _error == null; } }
    public bool IsFailure => !IsSuccess;

    public TSuccess? SuccessOrDefault() => this.Match(l => l, r => default);

    public Error? ErrorOrDefault() => this.Match(l => default, r => r);

    public static Result<TSuccess> Success(TSuccess success) => new(success);

    public static Result<TSuccess> Failure(Error error) => new(error);
}

public class Result : Result<bool>
{
    private Result() : base(true) { }

    public static Result Success() => new();
}
