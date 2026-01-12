using Microsoft.Extensions.DependencyInjection;

namespace Framework;

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

public interface IQueryMiddleware
{
    Task<TResult> Handle<TResult>(IQuery<TResult> query, Func<IQuery<TResult>, CancellationToken, Task<TResult>> next, CancellationToken ct);
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

        var middlewareRegister = serviceProvider.GetService(typeof(QueryMiddlewareRegistry)) as QueryMiddlewareRegistry;

        var pipeline = BuildPipeline(middlewareRegister, query, (q, t) => (Task<TResult>)handler.Handle(q, t), ct);

        return await pipeline.Invoke(query, ct);
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

    private Func<IQuery<TResult>, CancellationToken, Task<TResult>> BuildPipeline<TResult>(QueryMiddlewareRegistry? registry, IQuery<TResult> query, Func<IQuery<TResult>, CancellationToken, Task<TResult>> queryHandler, CancellationToken ct)
    {
        var pipeline = queryHandler;

        if (registry != null)
        {
            foreach (var middleware in registry.Middlewares.Reverse())
            {
                var mwInstance = serviceProvider.GetService(middleware) as IQueryMiddleware;
                if (mwInstance != null)
                {
                    var next = pipeline;
                    pipeline = (q, t) => mwInstance.Handle(query, next, ct);
                }
            }
        }

        return pipeline;
    }
}

public class QueryMiddlewareRegistry(ServiceCollection serviceCollection)
{
    private List<Type> _middlewares = new();
    internal IEnumerable<Type> Middlewares => _middlewares;

    public void Register<TMiddleware>() where TMiddleware : IQueryMiddleware
    {
        _middlewares.Add(typeof(TMiddleware));
        serviceCollection.AddScoped(typeof(TMiddleware));
    }
}
