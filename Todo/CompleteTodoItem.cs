using Framework;

public class CompleteTodoItemCommand : ICommand
{
    public CompleteTodoItemCommand(Guid id)
    {
        Id = TodoItemId.From(id);
    }

    internal TodoItemId Id { get; set; }
}

internal class CompleteTodoItemHandler(IStore store) : ICommandHandler<CompleteTodoItemCommand>
{
    public async Task Handle(CompleteTodoItemCommand command, CancellationToken ct)
    {
        var todoItem = await store.GetItem<TodoItem, TodoItemId>(command.Id);
        if (todoItem == null)
        {
            throw new ArgumentException("Given TodoItem does not exist");
        }

        todoItem.MarkAsComplete();
        await store.StoreItem(todoItem.Id, todoItem);
    }
}