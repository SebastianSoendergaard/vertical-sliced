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

    public T Match<T>(Func<TSuccess, T> onSuccess, Func<IEnumerable<Error>, T> onFailure)
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
        _ = _success ?? throw new InvalidOperationException($"{nameof(onSuccess)} can not be called with null");
        return onSuccess(_success);
    }

    public bool IsFailure { get { return _errors.Any(); } }

    public bool IsSuccess => !IsFailure;

    public TSuccess Value 
    { 
        get 
        {
            if (_success == null)
            {
                throw new InvalidOperationException("Value property can only be called on successfull results");
            }
            return _success; 
        } 
    }
    public IEnumerable<Error> Errors { get { return _errors; } }

    public static Result<TSuccess> Success(TSuccess success) => new(success);

    public static Result<TSuccess> Failure(Error error) => new([error]);
    public static Result<TSuccess> Failure(params Error[] errors) => new(errors);
    public static Result<TSuccess> Failure(params IEnumerable<Error>[] errors) => new(errors);

    public static implicit operator Result<TSuccess>(TSuccess success) => new(success);

    public static implicit operator Result<TSuccess>(Error error) => new([error]);
}

public class Result : Result<bool>
{
    private Result() : base(true) { }

    public static Result Success() => new();
}

public static class ResultExtension
{
    public static Result<TResult> Join<TSource, TResult>(this Result<TSource> source, Result<TResult> next)
    {
        return source.Match(
            success => next,
            errors => Result<TResult>.Failure(errors, next.Errors));
    }

    public static Result<TResult> Bind<TSuccess, TResult>(this Result<TSuccess> source, Func<TSuccess, TResult> next)
    {
        return source.Match(
            success => next(success),
            errors => Result<TResult>.Failure(errors));
    }

    public static Task<Result<TResult>> BindAsync<TSuccess, TResult>(this Result<TSuccess> source, Func<TSuccess, Task<Result<TResult>>> next)
    {
        return source.Match<Task<Result<TResult>>> (
            async success => await next(success),
            errors => Task.FromResult(Result<TResult>.Failure(errors)));
    }

    public static async Task<TResult> MatchAsync<TSuccess, TResult>(this Task<Result<TSuccess>> source, Func<TSuccess, Task<TResult>> onSuccess, Func<IEnumerable<Error>, Task<TResult>> onFailure)
    {
        return await (await source).Match(
            async success => await onSuccess(success),
            async errors => await onFailure(errors));
    }

    public static async Task<TResult> MatchAsync<TSuccess, TResult>(this Task<Result<TSuccess>> source, Func<TSuccess, TResult> onSuccess, Func<IEnumerable<Error>, TResult> onFailure)
    {
        return (await source).Match(
            success => onSuccess(success),
            errors => onFailure(errors));
    }
}
