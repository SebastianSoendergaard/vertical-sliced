using Framework;

public class AddTodoItemCommand : ICommand<Result<AddTodoItemResult>>
{
    private AddTodoItemCommand(TodoItemDescription description)
    {
        Description = description;
    }

    internal TodoItemDescription Description { get; private set; }

    public static Result<AddTodoItemCommand> Create(string description)
    { 
        return TodoItemDescription.Create(description)
            .Bind(d => new AddTodoItemCommand(d));
    }
}

public class AddTodoItemResult
{
    internal AddTodoItemResult(TodoItemId id)
    {
        Id = id.Value;
    }

    public Guid Id { get; private set; }
}

internal class AddTodoItemHandler(IStore store) : ICommandHandler<AddTodoItemCommand, Result<AddTodoItemResult>>
{
    public async Task<Result<AddTodoItemResult>> Handle(AddTodoItemCommand command, CancellationToken ct)
    {
        var todoItem = new TodoItem(TodoItemId.NewId(), command.Description);
        await store.StoreItem(todoItem.Id, todoItem);
        return new AddTodoItemResult(todoItem.Id);
    }
}