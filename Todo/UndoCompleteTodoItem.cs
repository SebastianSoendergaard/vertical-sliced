using Framework;

public class UndoCompleteTodoItemCommand : ICommand
{
    public UndoCompleteTodoItemCommand(Guid id)
    {
        Id = TodoItemId.From(id);
    }

    internal TodoItemId Id { get; set; }
}

internal class UndoCompleteTodoItemHandler(IStore store) : ICommandHandler<UndoCompleteTodoItemCommand>
{
    public async Task Handle(UndoCompleteTodoItemCommand command, CancellationToken ct)
    {
        var todoItem = await store.GetItem<TodoItem, TodoItemId>(command.Id);
        if (todoItem == null)
        {
            throw new ArgumentException("Given TodoItem does not exist");
        }

        todoItem.MarkAsIncomplete();
        await store.StoreItem(todoItem.Id, todoItem);
    }
}