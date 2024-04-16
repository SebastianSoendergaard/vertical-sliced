namespace Framework;

public interface ICommand
{ 
}

public interface ICommand<in TResult>
{
}

public interface ICommandHandler
{
    internal object Handle(object command, CancellationToken ct);
}

public interface ICommandHandler<in TCommand> : ICommandHandler where TCommand : ICommand
{
    object ICommandHandler.Handle(object command, CancellationToken ct) => Handle((TCommand)command, ct);
    Task Handle(TCommand command, CancellationToken ct);
}

public interface ICommandHandler<in TCommand, TResult> : ICommandHandler where TCommand : ICommand<TResult>
{
    object ICommandHandler.Handle(object command, CancellationToken ct) => Handle((TCommand)command, ct);
    Task<TResult> Handle(TCommand command, CancellationToken ct);
}

public interface ICommandDispatcher
{
    Task Dispatch(ICommand command, CancellationToken ct = new CancellationToken());
    Task<TResult> Dispatch<TResult>(ICommand<TResult> command, CancellationToken ct = new CancellationToken());
}

public class CommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
{
    public async Task Dispatch(ICommand query, CancellationToken ct)
    {
        var handlerType = GetHandlerType(query);

        var handler = serviceProvider.GetService(handlerType) as ICommandHandler;
        if (handler is null)
        {
            throw new InvalidOperationException($"No handler found for query: {query.GetType()}");
        }

        await (Task)handler.Handle(query, ct);
    }

    public async Task<TResult> Dispatch<TResult>(ICommand<TResult> command, CancellationToken ct)
    {
        var handlerType = GetHandlerType(command);

        var handler = serviceProvider.GetService(handlerType) as ICommandHandler;
        if (handler is null)
        {
            throw new InvalidOperationException($"No handler found for query: {command.GetType()}");
        }

        return await (Task<TResult>)handler.Handle(command, ct);
    }

    private Type GetHandlerType(ICommand command)
    {
        return typeof(ICommandHandler<>).MakeGenericType([command.GetType()]);
    }

    private Type GetHandlerType<TResult>(ICommand<TResult> command)
    {
        var interfaceType = typeof(ICommand<TResult>);
        var commandType = command.GetType();
        var returnType = commandType
            .GetInterfaces()
            .Where(i => i == interfaceType)
            .SelectMany(i => i.GenericTypeArguments)
        .First();

        return typeof(ICommandHandler<,>).MakeGenericType([commandType, returnType]);
    }
}