using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Framework;

public sealed record Error(string Code, string Description);

public interface IResult
{
    bool IsFailure { get; }

    bool IsSuccess { get; }
    public IEnumerable<Error> Errors { get; }
}

public class Result<TSuccess> : IResult
{
    private TSuccess? _success{ get; }
    private IList<Error> _errors { get; } = new List<Error>();

    protected Result(TSuccess success) 
    { 
        _success = success;
    }

    protected Result(params IEnumerable<Error>[] errors)
    {
        _errors = errors.SelectMany(x => x).ToList();
    }

    public T Match<T>(Func<TSuccess?, T> onSuccess, Func<IEnumerable<Error>, T> onFailure)
    {
        if (_errors.Any())
        {
            if (onFailure == null)
            {
                throw new ArgumentNullException(nameof(onFailure));
            }
            return onFailure(_errors);
        }

        if (onSuccess == null)
        {
            throw new ArgumentNullException(nameof(onSuccess));
        }
        return onSuccess(_success);
    }

    public bool IsFailure { get { return _errors.Any(); } }

    public bool IsSuccess => !IsFailure;

    public TSuccess SuccessResult 
    { 
        get 
        {
            if (_success == null)
            {
                throw new InvalidOperationException("Property can only be called on successfull results");
            }
            return _success; 
        } 
    }
    public IEnumerable<Error> Errors { get { return _errors; } }

    public static Result<TSuccess> Success(TSuccess success) => new(success);

    public static Result<TSuccess> Failure(Error error) => new(new[] { error });
    public static Result<TSuccess> Failure(params Error[] errors) => new(errors);
    public static Result<TSuccess> Failure(params IEnumerable<Error>[] errors) => new(errors);

    public static Result<TSuccess> Combined(Func<TSuccess> ifSuccess, params IResult[] results)
    {
        var allErrors = results.SelectMany(x => x.Errors);
        if (allErrors.Any())
        {
            return new(allErrors);
        }

        if (ifSuccess == null)
        {
            throw new ArgumentNullException(nameof(ifSuccess));
        }
        return new(ifSuccess());
    }
}

public class Result : Result<bool>
{
    private Result() : base(true) { }

    public static Result Success() => new();
}
