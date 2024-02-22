public interface IQuery<in TResult>
{ 
}

public interface IQueryHandler
{
    internal object Handle(object query, CancellationToken ct);
}

public interface IQueryHandler<in TQuery, TResult> : IQueryHandler where TQuery : IQuery<TResult>
{
    object IQueryHandler.Handle(object query, CancellationToken ct) => Handle((TQuery)query, ct);
    Task<TResult> Handle(TQuery query, CancellationToken ct);
}

public interface IQueryDispatcher
{
    Task<TResult> Dispatch<TResult>(IQuery<TResult> query, CancellationToken ct = new CancellationToken());
}

public class QueryDispatcher(IServiceProvider serviceProvider) : IQueryDispatcher
{
    public async Task<TResult> Dispatch<TResult>(IQuery<TResult> query, CancellationToken ct)
    {
        var handlerType = GetHandlerType(query);

        var handler = serviceProvider.GetService(handlerType) as IQueryHandler;
        if (handler is null)
        {
            throw new InvalidOperationException($"No handler found for query: {query.GetType()}");
        }

        return await (Task<TResult>)handler.Handle(query, ct);
    }

    private Type GetHandlerType<TResult>(IQuery<TResult> query)
    {
        var interfaceType = typeof(IQuery<TResult>);
        var queryType = query.GetType();
        var returnType = queryType
            .GetInterfaces()
            .Where(i => i == interfaceType)
            .SelectMany(i => i.GenericTypeArguments)
            .First();

        return typeof(IQueryHandler<,>).MakeGenericType([queryType, returnType]);
    }
}
