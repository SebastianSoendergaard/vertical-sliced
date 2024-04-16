using Framework;

public class AddTodoItemCommand : ICommand<AddTodoItemResult>
{
    private AddTodoItemCommand(TodoItemDescription description)
    {
        Description = description;
    }

    internal TodoItemDescription Description { get; private set; }

    public static Result<AddTodoItemCommand> Create(string description)
    { 
        var d = TodoItemDescription.Create(description);
        
        return d.IsFailure
            ? Result<AddTodoItemCommand>.Failure(new Error("", "")) // This should be combined error from all value objects
            : Result<AddTodoItemCommand>.Success(new AddTodoItemCommand(d.SuccessOrDefault()));
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

internal class AddTodoItemHandler(IStore store) : ICommandHandler<AddTodoItemCommand, AddTodoItemResult>
{
    public async Task<AddTodoItemResult> Handle(AddTodoItemCommand command, CancellationToken ct)
    {
        var todoItem = new TodoItem(TodoItemId.NewId(), command.Description);
        await store.StoreItem(todoItem.Id, todoItem);
        return new AddTodoItemResult(todoItem.Id);
    }
}