public class RemoveCompleteTodoItemCommand : ICommand
{
    public RemoveCompleteTodoItemCommand(Guid id)
    {
        Id = TodoItemId.From(id);
    }

    internal TodoItemId Id { get; set; }
}

internal class RemoveCompleteTodoItemHandler(IStore store) : ICommandHandler<RemoveCompleteTodoItemCommand>
{
    public async Task Handle(RemoveCompleteTodoItemCommand command, CancellationToken ct)
    {
        var todoItem = await store.GetItem<TodoItem, TodoItemId>(command.Id);
        if (todoItem == null)
        {
            throw new ArgumentException("Given TodoItem does not exist");
        }

        if (!todoItem.IsComplete)
        {
            throw new ArgumentException("The given TodoItem is not complete");
        }

        await store.RemoveItem(todoItem.Id);
    }
}